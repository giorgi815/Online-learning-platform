using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Authentication;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace OLP.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly RoleManager<IdentityRole<Guid>> _roles;
        private readonly IUserRepository _profiles;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenStore _refreshTokens;
        private readonly IEmailService _emailService;

        public AuthService(
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            RoleManager<IdentityRole<Guid>> roles,
            IUserRepository profiles,
            IConfiguration configuration,
            IRefreshTokenStore refreshTokens,
            IEmailService emailService)
        {
            _users = users;
            _signIn = signIn;
            _roles = roles;
            _profiles = profiles;
            _configuration = configuration;
            _refreshTokens = refreshTokens;
            _emailService = emailService;
        }

        public Task<RegistrationResponseDto> RegisterStudentAsync(RegisterDto dto)
        {
            return RegisterWithRoleAsync(dto, "Student");
        }

        public Task<RegistrationResponseDto> RegisterTeacherAsync(RegisterDto dto)
        {
            return RegisterWithRoleAsync(dto, "Teacher");
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var account = await _users.FindByEmailAsync(dto.Email);

            if (account is null)
                throw new UnauthorizedException("Invalid credentials");

            var result = await _signIn.CheckPasswordSignInAsync(account, dto.Password, lockoutOnFailure: true);

            if (!result.Succeeded)
                throw new UnauthorizedException("Invalid credentials or account locked");

            return await IssueTokens(account);
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            if (!RedisRefreshTokenStore.TryGetUserId(dto.RefreshToken, out var accountId))
                throw new UnauthorizedException("Invalid refresh token");

            var account = await _users.FindByIdAsync(accountId.ToString());
            if (account is null || await _users.IsLockedOutAsync(account) ||
                !await _signIn.CanSignInAsync(account))
                throw new UnauthorizedException("Invalid refresh token");

            var newRefreshToken = await _refreshTokens.RotateAsync(dto.RefreshToken);

            if (newRefreshToken is null)
                throw new UnauthorizedException("Invalid refresh token");

            var response = await MakeToken(account);
            response.RefreshToken = newRefreshToken;
            return response;
        }

        public Task LogoutAsync(Guid userId)
            => _refreshTokens.RevokeAllAsync(userId);

        public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new BadRequestException("New password do not match");

            var account = await _users.FindByIdAsync(userId.ToString())
                ?? throw new NotFoundException("Identity user not found");

            EnsureIdentity(await _users.ChangePasswordAsync(account, dto.CurrentPassword, dto.NewPassword));
            await _refreshTokens.RevokeAllAsync(account.Id);
        }

        public async Task ConfirmEmailAsync(ConfirmEmailDto dto)
        {
            var account = await _users.FindByIdAsync(dto.UserId.ToString());

            if (account is null)
                throw new BadRequestException("Invalid confirmation.");

            EnsureIdentity(await _users.ConfirmEmailAsync(account, DecodeToken(dto.Token)));
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var account = await _users.FindByEmailAsync(dto.Email);

            if (account is null || !account.EmailConfirmed)
                return;

            var token = await _users.GeneratePasswordResetTokenAsync(account);
            var link = BuildEmailLink("/reset-password", account.Id, token);

            await _emailService.SendEmailAsync(account.Email!, "Reset your OLP password",
                $"<p>Reset your password:</p><a href=\"{link}\">Reset password</a>");
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new BadRequestException("Passwords do not match.");

            var account = await _users.FindByIdAsync(dto.UserId.ToString());

            if (account is null || !account.EmailConfirmed)
                throw new BadRequestException("Invalid password reset.");

            EnsureIdentity(await _users.ResetPasswordAsync(account,
                DecodeToken(dto.Token), dto.NewPassword));

            await _refreshTokens.RevokeAllAsync(account.Id);
        }

        public async Task ResendConfirmationEmailAsync(ForgotPasswordDto dto)
        {
            var account = await _users.FindByEmailAsync(dto.Email);

            if (account is null || account.EmailConfirmed)
                return;

            await SendConfirmationAsync(account);
        }
        private async Task<RegistrationResponseDto> RegisterWithRoleAsync(RegisterDto dto, string role)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new BadRequestException("Passwords do not match.");

            if (await _users.FindByEmailAsync(dto.Email) is not null)
                throw new ConflictException("Email already registered.");

            if (!await _roles.RoleExistsAsync(role))
                throw new InvalidOperationException($"{role} role has not been seeded.");

            var account = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
            EnsureIdentity(await _users.CreateAsync(account, dto.Password));

            try
            {
                EnsureIdentity(await _users.AddToRoleAsync(account, role));

                var profile = new User
                {
                    ApplicationUserId = account.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };

                await _profiles.AddAsync(profile);
                await _profiles.SaveAsync();
            }
            catch
            {
                var savedProfile = await _profiles.GetAsync(u => u.ApplicationUserId == account.Id);
                if (savedProfile is not null)
                {
                    _profiles.Remove(savedProfile);
                    await _profiles.SaveAsync();
                }
                await _users.DeleteAsync(account);
                throw;
            }

            await SendConfirmationAsync(account);
            return new RegistrationResponseDto();
        }

        private static void EnsureIdentity(IdentityResult result)
        {
            if (!result.Succeeded) throw new BadRequestException(string.Join("; ", result.Errors.Select(e => e.Description)));
        }

        private async Task<AuthResponseDto> MakeToken(ApplicationUser account)
        {
            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            if (string.IsNullOrWhiteSpace(key) || Encoding.UTF8.GetByteCount(key) < 32)
                throw new InvalidOperationException("Configure Jwt:Key (at least 32 bytes) securely.");

            var expires = DateTime.UtcNow.AddMinutes(60);
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, account.Id.ToString()), new(ClaimTypes.Email, account.Email ?? "") };
            foreach (var role in await _users.GetRolesAsync(account))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expires,
                signingCredentials: credentials);

            return new AuthResponseDto
            {
                UserId = account.Id,
                Email = account.Email ?? "",
                AccessToken = new JwtSecurityTokenHandler().WriteToken(jwt),
                ExpiresAt = expires
            };
        }

        private async Task<AuthResponseDto> IssueTokens(ApplicationUser account)
        {
            var response = await MakeToken(account);
            response.RefreshToken = await _refreshTokens.CreateAsync(account.Id);
            return response;
        }

        private string BuildEmailLink(string route, Guid accountId, string token)
        {
            var frontendUrl = _configuration["Frontend:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "Frontend URL is missing.");

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

            var url = QueryHelpers.AddQueryString(
                frontendUrl.TrimEnd('/') + route,
                new Dictionary<string, string?>
                {
                    ["userId"] = accountId.ToString(),
                    ["token"] = encodedToken
                });

            return HtmlEncoder.Default.Encode(url);
        }

        private async Task SendConfirmationAsync(ApplicationUser account)
        {
            var token = await _users.GenerateEmailConfirmationTokenAsync(account);
            var link = BuildEmailLink("/confirm-email", account.Id, token);
            await _emailService.SendEmailAsync(account.Email!, "Confirm your OLP email",
                $"<p>Confirm your email:</p><a href=\"{link}\">Confirm email</a>");
        }

        private static string DecodeToken(string token)
        {
            try { return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)); }
            catch (FormatException) { throw new BadRequestException("Invalid token."); }
            catch (ArgumentException) { throw new BadRequestException("Invalid token."); }
        }
    }
}
