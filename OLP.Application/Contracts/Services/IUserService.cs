
using OLP.Application.DTOs.User;

namespace OLP.Application.Contracts.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserByIdAsync(Guid userId);

        Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();

        Task<UserResponseDto> UpdateProfileAsync(Guid userId, UpdateUserDto dto);

        Task<UserResponseDto> UpdateAvatarAsync(Guid userId, UpdateAvatarDto dto);

        Task<IEnumerable<UserResponseDto>> GetTeachersAsync();

        Task<IEnumerable<UserResponseDto>> GetStudentsAsync();
        Task<UserResponseDto> UploadAvatarAsync(Guid userId, Stream content, string fileName);
        Task<Stream> OpenAvatarAsync(Guid profileId);
    }
}
