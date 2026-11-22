using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Assignment;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/assignments")]
    public sealed class AssignmentController : ControllerBase
    {
        private readonly IAssignmentService _assignments;
        private readonly ICurrentUser _current;
        public AssignmentController(
            IAssignmentService assignment,
            ICurrentUser current)
        {
            _assignments = assignment;
            _current = current;
        }

        [HttpGet("by-course/{courseId:guid}")]
        public async Task<IActionResult> ByCourse(Guid courseId)
            => Ok(await _assignments.GetAssignmentsByCourseAsync(courseId, await _current.ProfileIdAsync()));


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
            => Ok(await _assignments.GetAssignmentByIdAsync(id, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAssignmentDto dto)
            => Ok(await _assignments.CreateAssignmentAsync(dto, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAssignmentDto dto)
            => Ok(await _assignments.UpdateAssignmentAsync(id, dto, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _assignments.DeleteAssignmentAsync(id, await _current.ProfileIdAsync());
            return NoContent();
        }
    }
}
