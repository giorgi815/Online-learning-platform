using Microsoft.AspNetCore.Identity;
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

namespace OLP.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signIn;
        private readonly RoleManager<IdentityRole<Guid>> _roles;
        private readonly IUserRepository _profiles;
        private readonly IConfiguration _configuration;
        IRefreshTokenStore _refreshTokens;

        public AuthService(
            UserManager<ApplicationUser> users,
            SignInManager<ApplicationUser> signIn,
            RoleManager<IdentityRole<Guid>> roles,
            IUserRepository profiles,
            IConfiguration configuration,
            IRefreshTokenStore refreshTokens)
        {
            _users = users;
            _signIn = signIn;
            _roles = roles;
            _profiles = profiles;
            _configuration = configuration;
            _refreshTokens = refreshTokens;
        }

        public Task<AuthResponseDto> RegisterStudentAsync(RegisterDto dto)
        {
            return RegisterWithRoleAsync(dto, "Student");
        }

        public Task<AuthResponseDto> RegisterTeacherAsync(RegisterDto dto)
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


        private async Task<AuthResponseDto> RegisterWithRoleAsync(RegisterDto dto, string role)
        {
            if (dto.Password != dto.ConfirmPassword)
                throw new BadRequestException("Passwords do not match.");

            if (await _users.FindByEmailAsync(dto.Email) is not null)
                throw new ConflictException("Email already registered.");

            if (!await _roles.RoleExistsAsync(role))
                throw new InvalidOperationException($"{role} role has not been seeded.");

            var account = new ApplicationUser { UserName = dto.Email, Email = dto.Email };
            var created = await _users.CreateAsync(account, dto.Password);

            try
            {
                var roleResult = await _users.CreateAsync(account, dto.Password);
                EnsureIdentity(roleResult);

                var profile = new User
                {
                    ApplicationUserId = account.Id,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName
                };

                await _profiles.AddAsync(profile);
                await _profiles.SaveAsync();

                return await IssueTokens(account);
            }
            catch
            {
                await _users.DeleteAsync(account);
                throw;
            }
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

    }
}
