using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Authentication
{
    public class ConfirmEmailDto
    {
        [Required] 
        public Guid UserId { get; set; }
        
        [Required, MaxLength(4096)] 
        public string Token { get; set; } = string.Empty;
    }
}
