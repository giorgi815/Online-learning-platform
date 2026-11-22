using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Authorize(Roles = "Student")]
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


        [HttpPut("lessons/{lessonId:guid}/complete")]
        public async Task<IActionResult> Complete(Guid lessonId)
        {
            await _progress.MarkLessonCompletedAsync(lessonId, await _current.ProfileIdAsync());

            return NoContent();
        }

        [HttpPut("lessons/{lessonId:guid}/incomplete")]
        public async Task<IActionResult> Incomplete(Guid lessonId)
        {
            await _progress.MarkLessonInCompleteAsync(lessonId, await _current.ProfileIdAsync());

            return NoContent();
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
    }
}
