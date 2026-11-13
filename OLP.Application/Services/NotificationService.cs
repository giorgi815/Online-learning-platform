using MapsterMapper;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Notification;

namespace OLP.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notifications;
        private readonly IUserRepository _profiles;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _notifications = notificationRepository;
            _profiles = userRepository;
            _mapper = mapper;
        }

        public Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteNotificationAsync(Guid notificationId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAllAsReadAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            throw new NotImplementedException();
        }
    }
}
