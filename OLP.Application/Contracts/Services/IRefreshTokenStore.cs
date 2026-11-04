
namespace OLP.Application.Contracts.Services
{
    public interface IRefreshTokenStore
    {
        Task<string> CreateAsync(Guid applicationUserId);
        Task<string?> RotateAsync(string oldToken);
        Task RevokeAllAsync(Guid applicationUserId);
    }
}
