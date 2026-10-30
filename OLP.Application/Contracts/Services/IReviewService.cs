
using OLP.Application.DTOs.Review;

namespace OLP.Application.Contracts.Services
{
    public interface IReviewService
    {
        Task<ReviewResponseDto> CreateReviewAsync(CreateReviewDto dto, Guid studentId);

        Task<ReviewResponseDto> UpdateReviewAsync(Guid reviewId, UpdateReviewDto dto, Guid studentId);

        Task DeleteReviewAsync(Guid reviewId, Guid studentId);

        Task<IEnumerable<ReviewResponseDto>> GetReviewsByCourseAsync(Guid courseId);

        Task<IEnumerable<ReviewResponseDto>> GetReviewsByStudentAsync(Guid studentId);

        Task<double> GetAvarageRatingAsync(Guid courseId);
    }
}
