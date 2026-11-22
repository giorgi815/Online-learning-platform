using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/enrollments")]
    public sealed class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollments;
        private readonly ICurrentUser _current;

        public EnrollmentController(
            IEnrollmentService enrollments,
            ICurrentUser current)
        {
            _enrollments = enrollments;
            _current = current;
        }


        [Authorize(Roles = "Student"), HttpPost("courses/{courseId:guid}")]
        public async Task<IActionResult> Enroll(Guid courseId)
            => Ok(await _enrollments.EnrollStudentAsync(courseId, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Student"), HttpDelete("courses/{courseId:guid}")]
        public async Task<IActionResult> Unenroll(Guid courseId)
        {
            await _enrollments.UnEnrollStudentAsync(courseId, await _current.ProfileIdAsync());

            return NoContent();
        }


        [Authorize(Roles = "Student"), HttpGet("mine")]
        public async Task<IActionResult> Mine()
            => Ok(await _enrollments.GetEnrollmentsByStudentAsync(await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpGet("courses/{courseId:guid}/students")]
        public async Task<IActionResult> ByCourse(Guid courseId)
            => Ok(await _enrollments.GetStudentsByCourseAsync(courseId, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Student"), HttpGet("courses/{courseId:guid}/mine")]
        public async Task<IActionResult> IsEnrolled(Guid courseId)
            => Ok(new { enrolled = await _enrollments.IsStudentEnrolledAsync(courseId, await _current.ProfileIdAsync()) });
    }
}
