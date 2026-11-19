using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.User;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;

namespace OLP.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _profiles;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService _files;
        private readonly IMapper _mapper;

        public UserService(IUserRepository profiles,
            UserManager<ApplicationUser> userManager,
            IFileStorageService files,
            IMapper mapper)
        {
            _profiles = profiles;
            _userManager = userManager;
            _files = files;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
        {
            var (items, _) = await _profiles.GetAllAsync(tracikng: false);

            return _mapper.Map<IEnumerable<UserResponseDto>>(items);
        }

        public Task<IEnumerable<UserResponseDto>> GetStudentsAsync() => ByRole("Student");

        public Task<IEnumerable<UserResponseDto>> GetTeachersAsync() => ByRole("Teacher");

        public async Task<UserResponseDto> GetUserByIdAsync(Guid userId) 
            =>_mapper.Map<UserResponseDto>(ServiceChecks.Found
                (await _profiles.GetAsync(
                    u => u.Id == userId,
                    tracking: false), "User"));

        public async Task<Stream> OpenAvatarAsync(Guid profileId)
        {
            var user = ServiceChecks.Found(await _profiles.GetAsync(u => u.Id == profileId, tracking: false), "User");

            if (string.IsNullOrWhiteSpace(user.Avatar))
                throw new NotFoundException("Avatar not found.");

            return await _files.OpenReadAsync(user.Avatar);
        }

        public async Task<UserResponseDto> UpdateAvatarAsync(Guid userId, UpdateAvatarDto dto)
        {
            var user = ServiceChecks.Found(await _profiles.GetAsync(u => u.Id == userId), "User");

            user.Avatar = dto.Avatar;

            await _profiles.SaveAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UpdateprofilAsync(Guid userId, UpdateAvatarDto dto)
        {
            var user = ServiceChecks.Found(await _profiles.GetAsync(u => u.Id == userId), "User");

            _mapper.Map(dto, user);

            await _profiles.SaveAsync();

            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> UploadAvatarAsync(Guid userId, Stream content, string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            
            ServiceChecks.Require(extension is ".png" or ".jpg" or ".jpeg", "Image required.");

            var user = ServiceChecks.Found(await _profiles.GetAsync(u => u.Id == userId), "User");
            var key = await _files.UploadAsync(content, fileName);

            user.Avatar = key;

            await _profiles.SaveAsync();
            return _mapper.Map<UserResponseDto>(user);
        }

        private async Task<IEnumerable<UserResponseDto>> ByRole(string role)
        {
            var accounts = await _userManager.GetUsersInRoleAsync(role);
            var ids = accounts.Select(a => a.Id).ToList();

            var (items, _) = await _profiles.GetAllAsync(
                filter: u => ids.Contains(u.ApplicationUserId),
                tracikng: false);

            return _mapper.Map<List<UserResponseDto>>(items.ToList());
        }
    }
}
