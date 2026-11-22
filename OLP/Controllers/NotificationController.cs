using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Notification;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/notifications")]
    public sealed class NotificationController : ControllerBase
    {
        private readonly INotificationService _notifications;
        private readonly ICurrentUser _current;

        public NotificationController(
            INotificationService notifications,
            ICurrentUser current)
        {
            _notifications = notifications;
            _current = current;
        }


        [Authorize(Roles = "Admin"), HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateNotificationDto dto)
            => Ok(await _notifications.CreateNotificationAsync(dto));


        [HttpGet("mine")]
        public async Task<IActionResult> Mine()
            => Ok(await _notifications.GetUserNotificationsAsync(await _current.ProfileIdAsync()));


        [HttpPut("{id:guid}/read")]
        public async Task<IActionResult> MarkRead(Guid id)
        {
            await _notifications.MarkAsReadAsync(id, await _current.ProfileIdAsync());

            return NoContent();
        }


        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            await _notifications.MarkAllAsReadAsync(await _current.ProfileIdAsync());

            return NoContent();
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _notifications.DeleteNotificationAsync(id, await _current.ProfileIdAsync());

            return NoContent();
        }
    }
}
