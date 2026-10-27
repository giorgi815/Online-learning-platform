using Microsoft.AspNetCore.Identity;

namespace OLP.Domain.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public User? User { get; set; }
    }
}
