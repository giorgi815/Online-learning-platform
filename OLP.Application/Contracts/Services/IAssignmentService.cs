
using OLP.Application.DTOs.Assignment;

namespace OLP.Application.Contracts.Services
{
    public interface IAssignmentService
    {
        Task<AssignmentResponseDto> CreateAssignmentAsync(CreateAssignmentDto dto, Guid teacherId);

        Task<AssignmentResponseDto> UpdateAssignmentAsync(Guid assignmentId, UpdateAssignmentDto dto, Guid teacherId);

        Task DeleteAssignmentAsync(Guid assignmentId, Guid teacherId);

        Task<AssignmentResponseDto> GetAssignmentByIdAsync(Guid assignmentId, Guid actorId);

        Task<IEnumerable<AssignmentResponseDto>> GetAssignmentsByCourseAsync(Guid courseId, Guid actorId);


    }
}
