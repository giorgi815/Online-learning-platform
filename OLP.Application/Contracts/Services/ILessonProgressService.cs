
using OLP.Application.DTOs.LessonProgress;

namespace OLP.Application.Contracts.Services
{
    public interface ILessonProgressService
    {
        Task MarkLessonCompletedAsync(Guid lessonId, Guid studentId);

        Task MarkLessonInCompleteAsync(Guid lessonId, Guid studentId);

        Task<IEnumerable<LessonProgressResponseDto>> GetStudentProgressAsync(Guid studentId, Guid courseId);

        Task<double> GetCourseProgressAsync(Guid studentId, Guid courseId);

        Task<bool> IsLessonCompletedAsync(Guid lessonId, Guid studentId);
    }
}
