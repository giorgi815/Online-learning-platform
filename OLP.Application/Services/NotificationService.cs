using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Notification;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;

namespace OLP.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notifications;
        private readonly IUserRepository _profiles;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public NotificationService(INotificationRepository notificationRepository,
            IUserRepository userRepository,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _notifications = notificationRepository;
            _userManager = userManager;
            _profiles = userRepository;
            _mapper = mapper;
        }

        public async Task<NotificationResponseDto> CreateNotificationAsync(CreateNotificationDto dto)
        {
            var email = dto.Email.Trim();
            var account = await _userManager.FindByEmailAsync(email);
            if (account is null)
                throw new NotFoundException("Recipient user not found.");

            var profile = await _profiles.GetAsync(
                u => u.ApplicationUserId == account.Id,
                tracking: false);
            if (profile is null)
                throw new NotFoundException("Recipient profile not found.");

            var notification = new Notification
            {
                UserId = profile.Id,
                Title = dto.Title,
                Message = dto.Message
            };

            await _notifications.AddAsync(notification);
            await _notifications.SaveAsync();

            return _mapper.Map<NotificationResponseDto>(notification);
        }

        public async Task DeleteNotificationAsync(Guid notificationId, Guid userId)
        {
            var entity = ServiceChecks.Found(await _notifications.GetAsync(n => n.Id == notificationId && n.UserId == userId), "Notification");

            _notifications.Remove(entity);
            await _notifications.SaveAsync();
        }

        public async Task<IEnumerable<NotificationResponseDto>> GetUserNotificationsAsync(Guid userId)
        {
            var (items, _) = await _notifications.GetAllAsync(filter: n => n.UserId == userId, orderBy: n => n.CreatedAt, ascending: false, tracikng: false);

            return _mapper.Map<IEnumerable<NotificationResponseDto>>(items);
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var (items, _) = await _notifications.GetAllAsync(filter: n => n.UserId == userId && !n.IsRead);

            foreach (var item in items)
                item.IsRead = true;

            await _notifications.SaveAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId, Guid userId)
        {
            var entity = ServiceChecks.Found(await _notifications.GetAsync(n => n.Id == notificationId && n.UserId == userId), "Notification");

            entity.IsRead = true;

            await _notifications.SaveAsync();
        }
    }
}
