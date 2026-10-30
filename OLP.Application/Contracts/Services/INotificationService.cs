
using OLP.Application.DTOs.Notification;

namespace OLP.Application.Contracts.Services
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationDto dto);

        Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(Guid userId);

        Task MarkAsReadAsync(Guid notificationId, Guid userId);

        Task MarkAllAsReadAsync(Guid userId);

        Task DeleteNotificationAsync(Guid notificationId, Guid userId);
    }
}
