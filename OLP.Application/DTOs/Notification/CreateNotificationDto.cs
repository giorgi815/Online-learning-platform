using OLP.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Notification
{
    public class CreateNotificationDto
    {
        [NotEmptyGuid]
        public Guid UserId { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000, MinimumLength = 3)]
        public string Message { get; set; } = string.Empty;
    }
}
