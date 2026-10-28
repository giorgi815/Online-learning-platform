using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Authentication
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
