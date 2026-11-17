using OLP.Application.DTOs.Admin;

namespace OLP.Application.Contracts.Services
{
    public interface IAdminService
    {
        Task<PlatformStatisticsDto> GetStatisticsAsync(Guid adminAccountId);
        Task GrantTeacherRoleAsync(Guid adminAccountId, Guid targetUserId);
    }
}
