using OLP.Application.Contracts.Persistence;
using OLP.Application.Exceptions;
using System.Security.Claims;

namespace OLP.Security
{
    public class CurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _http;
        private readonly IUserRepository _profiles;
        private Guid? _profileId;

        public CurrentUser(IHttpContextAccessor http, IUserRepository profiles)
        {
            _http = http;
            _profiles = profiles;
        }

        public Guid AccountId
        {
            get
            {
                var principal = _http.HttpContext?.User;
                var value = principal?.FindFirstValue(ClaimTypes.NameIdentifier);
                if (principal?.Identity?.IsAuthenticated != true || !Guid.TryParse(value, out var id))
                    throw new UnauthorizedException("Valid authentication is required.");
                return id;
            }
        }

        public async Task<Guid> ProfileIdAsync()
        {
            if (_profileId.HasValue) return _profileId.Value;
            var accountId = AccountId;
            var profile = await _profiles.GetAsync(u => u.ApplicationUserId == accountId, tracking: false)
                ?? throw new NotFoundException("Domain user profile not found.");
            _profileId = profile.Id;
            return profile.Id;
        }
    }
}
