using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Authentication
{
    public class ForgotPasswordDto
    {
        [Required, EmailAddress, MaxLength(256)] 
        public string Email { get; set; } = string.Empty;
    }
}
