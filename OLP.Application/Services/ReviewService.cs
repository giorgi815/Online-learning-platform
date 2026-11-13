using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Review;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;
using System.Linq.Expressions;

namespace OLP.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviews;
        private readonly ICourseRepository _courses;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper)
        {
            _reviews = reviewRepository;
            _courses = courseRepository;
            _enrollments = enrollmentRepository;
            _mapper = mapper;
        }

        public async Task<ReviewResponseDto> CreateReviewAsync(CreateReviewDto dto, Guid studentId)
        {
            Validate(dto.Rating);
            if(!await _courses.ExistsAsync(c => c.Id == dto.CourseId)) 
                throw new NotFoundException("Course not found.");

            if(!await _enrollments.ExistsAsync(e => e.CourseId == dto.CourseId && e.StudentId == studentId))
                throw new ForbiddenException("Enroll before reviewing.");

            if(await _reviews.ExistsAsync(r => r.CourseId == dto.CourseId && r.StudentId == studentId))
                throw new ConflictException("Course already reviewed.");

            var entity = _mapper.Map<Domain.Entities.Review>(dto);
            entity.StudentId = studentId;

            await _reviews.AddAsync(entity);
            await _reviews.SaveAsync();

            return await Response(entity.Id);
        }

        public async Task DeleteReviewAsync(Guid reviewId, Guid studentId)
        {
            var entity = ServiceChecks.Found(_reviews.GetAsync(r => r.Id == reviewId).Result, "Review");
            ServiceChecks.Owner(entity.StudentId, studentId);

            _reviews.Remove(entity);
            await _reviews.SaveAsync();
        }

        public async Task<double> GetAvarageRatingAsync(Guid courseId)
        {
            var (items, _) = await _reviews.GetAllAsync(filter: r => r.CourseId == courseId, tracikng: false);

            var ratings = items.Select(r => r.Rating);

            return ratings.Any() ?  ratings.Average() : 0;

        }

        public Task<IEnumerable<ReviewResponseDto>> GetReviewsByCourseAsync(Guid courseId) => List(r => r.CourseId == courseId);

        public Task<IEnumerable<ReviewResponseDto>> GetReviewsByStudentAsync(Guid studentId) => List(r => r.StudentId == studentId);

        public async Task<ReviewResponseDto> UpdateReviewAsync(Guid reviewId, UpdateReviewDto dto, Guid studentId)
        {
            Validate(dto.Rating);
            var entity = ServiceChecks.Found(await _reviews.GetAsync(r => r.Id == reviewId), "Review");

            ServiceChecks.Owner(entity.StudentId, studentId);
            _mapper.Map(dto, entity);

            await _reviews.SaveAsync();
            return await Response(reviewId);
        }

        private static void Validate(int rating) => ServiceChecks.Require(rating is >= 1 and <= 5, "Rating must be 1–5.");

        private async Task<ReviewResponseDto> Response(Guid id)
        => _mapper.Map<ReviewResponseDto>(
            ServiceChecks.Found(
                await _reviews.GetAsync(
                    r => r.Id == id,
                    tracking: false,
                    include: q => q.Include(r => r.Student)), "Review"));

        private async Task<IEnumerable<ReviewResponseDto>> List(Expression<Func<Review, bool>> predicate)
        {
            var (items, _) = await _reviews.GetAllAsync(
                filter: predicate,
                tracikng: false,
                includes: [r => r.Student]);

            return _mapper.Map<List<ReviewResponseDto>>(items.ToList());
        }
    }
}
