using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Submission;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/submissions")]
    public sealed class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _submissions;
        private readonly ICurrentUser _current;

        public SubmissionController(
            ISubmissionService submissions,
            ICurrentUser current)
        {
            _submissions = submissions;
            _current = current;
        }


        [Authorize(Roles = "Student"), HttpPost]
        public async Task<IActionResult> Submit([FromBody] CreateSubmissionDto dto)
            => Ok(await _submissions.SubmitAssignmentAsync(dto, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Student"), HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSubmissionDto dto)
            => Ok(await _submissions.UpdateSubmissionAsync(id, dto, await _current.ProfileIdAsync()));


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
            => Ok(await _submissions.GetSubmissionByIdAsync(id, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpGet("by-assignment/{assignmentId:guid}")]
        public async Task<IActionResult> ByAssignment(Guid assignmentId)
            => Ok(await _submissions.GetSubmissionsByAssignmentAsync(assignmentId, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Student"), HttpGet("mine")]
        public async Task<IActionResult> Mine()
            => Ok(await _submissions.GetSubmissionsByStudentAsync(await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpPut("{id:guid}/grade")]
        public async Task<IActionResult> Grade(Guid id, [FromBody] GradeSubmissionDto dto)
            => Ok(await _submissions.GradeSubmissionAsync(id, dto, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Student"), HttpPost("{id:guid}/attachments")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(25_000_000)]
        public async Task<IActionResult> Upload(Guid id, IFormFile file)
        {
            if (file is null || file.Length == 0 || file.Length > 25_000_000)
                return BadRequest("A nonempty file (max 25 MB) is required.");

            await using var stream = file.OpenReadStream();
            
            return Ok(await _submissions.UploadAttachmentAsync(id, await _current.ProfileIdAsync(), stream, file.FileName));
        }

        [HttpGet("{id:guid}/attachments")]
        public async Task<IActionResult> Download(Guid id, [FromQuery] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest("File key required.");
            
            var stream = await _submissions.OpenAttachmentAsync(id, await _current.ProfileIdAsync(), key);
            
            return File(stream, "application/octet-stream", "submission-attachment", enableRangeProcessing: true);
        }
    }
}
