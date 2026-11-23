using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Lesson;
using OLP.Application.Services;
using OLP.Security;

namespace OLP.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/progress")]
    public sealed class LessonProgressController : ControllerBase
    {
        private readonly ILessonProgressService _progress;
        private readonly ICurrentUser _current;

        public LessonProgressController(
            ILessonProgressService progress,
            ICurrentUser current)
        {
            _progress = progress;
            _current = current;
        }


        [HttpGet("courses/{courseId:guid}/lessons")]
        public async Task<IActionResult> ByCourse(Guid courseId)
            => Ok(await _progress.GetStudentProgressAsync(await _current.ProfileIdAsync(), courseId));


        [HttpGet("courses/{courseId:guid}/percentage")]
        public async Task<IActionResult> Percentage(Guid courseId)
            => Ok(new { percentage = await _progress.GetCourseProgressAsync(await _current.ProfileIdAsync(), courseId) });


        [HttpGet("lessons/{lessonId:guid}/completed")]
        public async Task<IActionResult> IsCompleted(Guid lessonId)
            => Ok(new { completed = await _progress.IsLessonCompletedAsync(lessonId, await _current.ProfileIdAsync()) });

        [HttpPost("complete-by-teacher")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> CompleteByTeacher([FromBody] MarkLessonCompleteDto dto)
        {
            var teacherId = await _current.ProfileIdAsync();

            await _progress.MarkCompleteByTeacherAsync(
                dto.LessonId,
                dto.StudentId,
                teacherId);

            return Ok(new
            {
                message = "Lesson marked as completed successfully."
            });
        }
    }
}
