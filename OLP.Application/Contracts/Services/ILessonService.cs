
using OLP.Application.DTOs.Lesson;

namespace OLP.Application.Contracts.Services
{
    public interface ILessonService
    {
        Task<LessonResponseDto> CreateLessonAsync(CreateLessonDto dto, Guid teacherId);

        Task<LessonResponseDto> UpdateLessonAsync(Guid lessonId, UpdateLessonDto dto, Guid teacherId);

        Task DeleteLessonAsync(Guid lessonId, Guid teacherId);

        Task<LessonResponseDto> GetLessonByIdAsync(Guid lessonId, Guid actorId);

        Task<IEnumerable<LessonResponseDto>> GetLessonByCourseAsync(Guid courseId, Guid actorId);

        Task<IEnumerable<LessonResponseDto>> GetAllLessonsAsync();

        Task<LessonResponseDto> UploadAttachmentAsync(Guid lessonId, Guid teacherId, Stream content, string fileName);
        Task<Stream> OpenAttachmentAsync(Guid lessonId, Guid actorId, string key);

    }
}
