using OLP.Application.DTOs.Certificate;

namespace OLP.Application.Contracts.Services
{
    public interface ICertificateService
    {
        Task<CertificateResponseDto> IssueAsync(Guid studentId, Guid courseId);
        Task<CertificateResponseDto> GetAsync(Guid studentId, Guid certificateId);
    }
}
