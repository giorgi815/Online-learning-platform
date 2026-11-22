using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Authentication;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly ICurrentUser _current;
        public AuthController(
            IAuthService auth,
            ICurrentUser current)
        {
            _auth = auth;
            _current = current;
        }

        [AllowAnonymous, HttpPost("register/student")]
        public async Task<IActionResult> RegisterStudent([FromBody] RegisterDto dto)
            => Ok(await _auth.RegisterStudentAsync(dto));


        [AllowAnonymous, HttpPost("register/teacher")]
        public async Task<IActionResult> RegisterTeacher([FromBody] RegisterDto dto)
            => Ok(await _auth.RegisterTeacherAsync(dto));


        [AllowAnonymous, HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
            => Ok(await _auth.LoginAsync(dto));


        [AllowAnonymous, HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
            => Ok(await _auth.RefreshTokenAsync(dto));



        [Authorize, HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _auth.LogoutAsync(_current.AccountId);
            return NoContent();
        }

        [Authorize, HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            await _auth.ChangePasswordAsync(_current.AccountId, dto);
            return NoContent();
        }

        [AllowAnonymous, HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            await _auth.ConfirmEmailAsync(dto);
            return Ok(new { message = "Email confirmed. You can now log in." });
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmailFromLink(
    [FromQuery] Guid userId,
    [FromQuery] string token)
        {
            await _auth.ConfirmEmailAsync(new ConfirmEmailDto
            {
                UserId = userId,
                Token = token
            });

            return Content(
                "<h2>Email confirmed successfully!</h2>" +
                "<p>You can now log in to OLP.</p>",
                "text/html");
        }

        [AllowAnonymous, HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            await _auth.ForgotPasswordAsync(dto);
            return Ok(new { message = "If the account exists, a password reset email has been sent." });
        }

        [AllowAnonymous, HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            await _auth.ResetPasswordAsync(dto);
            return Ok(new { message = "Password changed." });
        }

        [AllowAnonymous, HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ForgotPasswordDto dto)
        {
            await _auth.ResendConfirmationEmailAsync(dto);
            return Ok(new { message = "If confirmation is needed, a new email has been sent." });
        }
    }
}
