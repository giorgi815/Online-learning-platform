
using OLP.Application.DTOs.Lesson;

namespace OLP.Application.Contracts.Services
{
    public interface ILessonService
    {
        Task<LessonResponseDto> CreateLessonAsync(CreateLessonDto dto, Guid teacherId);

        Task<LessonResponseDto> UpdateLessonAsync(Guid lessonId, UpdateLessonDto dto, Guid teacherId);

        Task DeleteLessonAsync(Guid lessonId, Guid teacherId);

        Task<LessonResponseDto> GetLessonByIdAsync(Guid lessonId);

        Task<IEnumerable<LessonResponseDto>> GetLessonByCourseAsync(Guid courseId);

        Task<IEnumerable<LessonResponseDto>> GetAllLessonsAsync();

    }
}
