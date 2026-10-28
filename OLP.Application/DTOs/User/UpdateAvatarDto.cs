using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.User
{
    public class UpdateAvatarDto
    {
        [Required]
        [MaxLength(2000)]
        public string Avatar { get; set; } = string.Empty;
    }
}
