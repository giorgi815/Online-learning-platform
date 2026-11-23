using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Lesson;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/lessons")]
    public sealed class LessonController : ControllerBase
    {
        private readonly ILessonService _lessons;
        private readonly ICurrentUser _current;

        public LessonController(
            ILessonService lessons,
            ICurrentUser current)
        {
            _lessons = lessons;
            _current = current;
        }


        [HttpGet("by-course/{courseId:guid}")]
        public async Task<IActionResult> ByCourse(Guid courseId)
            => Ok(await _lessons.GetLessonByCourseAsync(courseId, await _current.ProfileIdAsync()));


        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id)
            => Ok(await _lessons.GetLessonByIdAsync(id, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLessonDto dto)
            => Ok(await _lessons.CreateLessonAsync(dto, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLessonDto dto)
            => Ok(await _lessons.UpdateLessonAsync(id, dto, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Teacher"), HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _lessons.DeleteLessonAsync(id, await _current.ProfileIdAsync());
            return NoContent();
        }



        [Authorize(Roles = "Teacher"), HttpPost("{id:guid}/attachments")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(25_000_000)]
        public async Task<IActionResult> Upload(Guid id, IFormFile file)
        {
            if (file is null || file.Length == 0 || file.Length > 25_000_000)
                return BadRequest("A nonempty file (max 25 MB) is required.");

            await using var stream = file.OpenReadStream();

            return Ok(await _lessons.UploadAttachmentAsync(id, await _current.ProfileIdAsync(), stream, file.FileName));
        }

        [HttpGet("{id:guid}/attachments")]
        public async Task<IActionResult> Download(Guid id, [FromQuery] string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return BadRequest("File key required.");

            var stream = await _lessons.OpenAttachmentAsync(
                id,
                await _current.ProfileIdAsync(),
                key);

            var contentType = Path.GetExtension(key).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".zip" => "application/zip",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            return File(
                stream,
                contentType,
                Path.GetFileName(key),
                enableRangeProcessing: true);
        }
    }
}
