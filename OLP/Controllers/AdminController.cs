using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    public sealed class AdminController : ControllerBase
    {
        private readonly IAdminService _admin;
        private readonly ICurrentUser _current;

        public AdminController(
            IAdminService admin,
            ICurrentUser current)
        {
            _admin = admin;
            _current = current;
        }


        [HttpGet("statistics")]
        public async Task<IActionResult> Statistics()
            => Ok(await _admin.GetStatisticsAsync(_current.AccountId));


        [HttpPost("users/{targetUserId:guid}/teacher-role")]
        public async Task<IActionResult> GrantTeacherRole(Guid targetUserId)
        {
            await _admin.GrantTeacherRoleAsync(_current.AccountId, targetUserId);

            return NoContent();
        }
    }
}
