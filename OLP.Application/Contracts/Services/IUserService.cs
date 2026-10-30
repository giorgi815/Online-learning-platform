
using OLP.Application.DTOs.User;

namespace OLP.Application.Contracts.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(Guid userId);

        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();

        Task<UserResponseDto> UpdateprofilAsync(Guid userId, UpdateAvatarDto dto);

        Task<IEnumerable<UserResponseDto>> GetTeachersAsync();

        Task<IEnumerable<UserResponseDto>> GetStudentsAsync();
    }
}
