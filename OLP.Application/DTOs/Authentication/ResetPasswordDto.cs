using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Authentication
{
    public class ResetPasswordDto
    {
        [Required] 
        public Guid UserId { get; set; }
        
        [Required, MaxLength(4096)] 
        public string Token { get; set; } = string.Empty;
        
        [Required, StringLength(100, MinimumLength = 8)] 
        public string NewPassword { get; set; } = string.Empty;
        
        [Required, Compare(nameof(NewPassword))] 
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
