
using OLP.Application.DTOs.Course;

namespace OLP.Application.Contracts.Services
{
    public interface ICourseService
    {
        Task<CourseResponseDto> CreateCourseAsync(CreateCourseDto dto, Guid teacherId);

        Task<CourseResponseDto> UpdateCourseAsync(UpdateCourseDto dto, Guid courseId, Guid teacherId);

        Task DeleteCourseAsync(Guid courseId, Guid teacherId);

        Task<CourseResponseDto> GetCourseByIdAsync(Guid courseId);

        Task<IEnumerable<CourseResponseDto>> GetAllCourseAsync();

        Task<IEnumerable<CourseResponseDto>> GetCourseByTeacherIdAsync(Guid teacherId);

        Task<IEnumerable<CourseResponseDto>> GetCourseByCategoryAsync(Guid categoryId);

        Task PublishCourse(Guid courseId, Guid teacherId);
    }
}
