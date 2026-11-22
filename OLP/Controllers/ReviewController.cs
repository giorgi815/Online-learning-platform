using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Review;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    public sealed class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviews;
        private readonly ICurrentUser _current;

        public ReviewController(
            IReviewService reviews,
            ICurrentUser current)
        {
            _reviews = reviews;
            _current = current;
        }


        [AllowAnonymous, HttpGet("by-course/{courseId:guid}")]
        public async Task<IActionResult> ByCourse(Guid courseId)
            => Ok(await _reviews.GetReviewsByCourseAsync(courseId));


        [AllowAnonymous, HttpGet("by-student/{studentId:guid}")]
        public async Task<IActionResult> ByStudent(Guid studentId)
            => Ok(await _reviews.GetReviewsByStudentAsync(studentId));


        [AllowAnonymous, HttpGet("by-course/{courseId:guid}/average")]
        public async Task<IActionResult> Average(Guid courseId)
            => Ok(new { averageRating = await _reviews.GetAvarageRatingAsync(courseId) });


        [Authorize(Roles = "Student"), HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
            => Ok(await _reviews.CreateReviewAsync(dto, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Student"), HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReviewDto dto)
            => Ok(await _reviews.UpdateReviewAsync(id, dto, await _current.ProfileIdAsync()));


        [Authorize(Roles = "Student"), HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _reviews.DeleteReviewAsync(id, await _current.ProfileIdAsync());
            
            return NoContent();
        }
    }
}
