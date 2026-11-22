using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.User;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Route("api/users")]
    public sealed class UserController : ControllerBase
    {
        private readonly IUserService _users;
        private readonly ICurrentUser _current;

        public UserController(
            IUserService users,
            ICurrentUser current)
        {
            _users = users;
            _current = current;
        }


        [AllowAnonymous, HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
            => Ok(await _users.GetUserByIdAsync(id));


        [AllowAnonymous, HttpGet("teachers")]
        public async Task<IActionResult> Teachers()
            => Ok(await _users.GetTeachersAsync());


        [Authorize(Roles = "Admin"), HttpGet("students")]
        public async Task<IActionResult> Students()
            => Ok(await _users.GetStudentsAsync());


        [Authorize(Roles = "Admin"), HttpGet]
        public async Task<IActionResult> All()
            => Ok(await _users.GetAllUsersAsync());


        [Authorize, HttpGet("me")]
        public async Task<IActionResult> Me()
            => Ok(await _users.GetUserByIdAsync(await _current.ProfileIdAsync()));


        [Authorize, HttpPut("me")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserDto dto)
            => Ok(await _users.UpdateProfileAsync(await _current.ProfileIdAsync(), dto));


        [Authorize, HttpPost("me/avatar")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file is null || file.Length == 0 || file.Length > 5_000_000)
                return BadRequest("A nonempty image (max 5 MB) is required.");

            await using var stream = file.OpenReadStream();
            
            return Ok(await _users.UploadAvatarAsync(await _current.ProfileIdAsync(), stream, file.FileName));
        }

        [AllowAnonymous, HttpGet("{id:guid}/avatar")]
        public async Task<IActionResult> Avatar(Guid id)
        {
            var stream = await _users.OpenAvatarAsync(id);
            
            return File(stream, "application/octet-stream", "avatar", enableRangeProcessing: true);
        }
    }
}
