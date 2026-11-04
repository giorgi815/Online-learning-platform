
using OLP.Application.DTOs.Authentication;

namespace OLP.Application.Contracts.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterStudentAsync(RegisterDto dto);

        Task<AuthResponseDto> RegisterTeacherAsync(RegisterDto dto);

        Task<AuthResponseDto> LoginAsync(LoginDto dto);

        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto dto);

        Task LogoutAsync(Guid userId);

        Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    }
}
