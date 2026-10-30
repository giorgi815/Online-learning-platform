
using OLP.Application.DTOs.Submission;

namespace OLP.Application.Contracts.Services
{
    public interface ISubmissionService
    {
        Task<SubmissionResponseDto> SubmitAssignmentAsync(CreateSubmissionDto dto, Guid studentId);

        Task<SubmissionResponseDto> UpdateSubmissionAsync(Guid submissionId, UpdateSubmissionDto dto, Guid studentId);

        Task<SubmissionResponseDto> GetSubmissionByIdAsync(Guid submissionId, Guid requestId);

        Task<IEnumerable<SubmissionResponseDto>> GetSubmissionsByAssignmentAsync(Guid assignmentId, Guid teacherId);

        Task<IEnumerable<SubmissionResponseDto>> GetSubmissionsByStudentAsync(Guid studentId);

        Task<SubmissionResponseDto> GradeSubmissionAsync(Guid submissionId, GradeSubmissionDto dto, Guid teacherId);
    }
}
