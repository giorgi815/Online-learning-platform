using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Course;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Route("api/courses")]
    public sealed class CourseController : ControllerBase
    {
        private readonly ICourseService _courses;
        private readonly ICurrentUser _current;

        public CourseController(
            ICourseService courses,
            ICurrentUser current)
        {
            _courses = courses;
            _current = current;
        }


        [AllowAnonymous, HttpGet]
        public async Task<IActionResult> GetAll() 
            => Ok(await _courses.GetAllCourseAsync());


        [AllowAnonymous, HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id) 
            => Ok(await _courses.GetCourseByIdAsync(id));


        [AllowAnonymous, HttpGet("by-category/{categoryId:guid}")]
        public async Task<IActionResult> ByCategory(Guid categoryId)
            => Ok(await _courses.GetCourseByCategoryAsync(categoryId));


        [AllowAnonymous, HttpGet("by-teacher/{teacherId:guid}")]
        public async Task<IActionResult> ByTeacher(Guid teacherId)
        {
            var actorId = User.Identity?.IsAuthenticated == true ? await _current.ProfileIdAsync() : Guid.Empty;

            return Ok(await _courses.GetCourseByTeacherIdAsync(teacherId, actorId));
        }

        [Authorize(Roles = "Teacher"), HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCourseDto dto)
            => Ok(await _courses.CreateCourseAsync(dto, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCourseDto dto)
            => Ok(await _courses.UpdateCourseAsync(dto, id, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpPost("{id:guid}/publish")]
        public async Task<IActionResult> Publish(Guid id)
        {
            await _courses.PublishCourse(id, await _current.ProfileIdAsync());

            return NoContent();
        }

        [Authorize(Roles = "Teacher"), HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _courses.DeleteCourseAsync(id, await _current.ProfileIdAsync());
            
            return NoContent();
        }
    }
}
