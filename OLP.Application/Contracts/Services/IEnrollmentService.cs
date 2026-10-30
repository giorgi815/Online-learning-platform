
using OLP.Application.DTOs.Enrollment;

namespace OLP.Application.Contracts.Services
{
    public interface IEnrollmentService
    {
        Task<EnrollmentResponseDto> EnrollStudentAsync(Guid courseId, Guid studentId);

        Task UnEnrollStudentAsync(Guid courseId, Guid studentId);

        Task<IEnumerable<EnrollmentResponseDto>> GetEnrollmentsByStudentAsync(Guid studentId);

        Task<IEnumerable<EnrollmentResponseDto>> GetStudentsByCourseAsync(Guid courseId, Guid requestId);

        Task<bool> IsStudentEnrolledAsync(Guid courseId, Guid studentId);
    }
}
