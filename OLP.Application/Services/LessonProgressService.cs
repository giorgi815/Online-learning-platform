using MapsterMapper;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.LessonProgress;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;

namespace OLP.Application.Services
{
    public class LessonProgressService : ILessonProgressService
    {
        private readonly ILessonProgressRepository _progress;
        private readonly ILessonRepository _lessons;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IMapper _mapper;

        public LessonProgressService(ILessonProgressRepository lessonProgressRepository,
            ILessonRepository lessonRepository,
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper)
        {
            _progress = lessonProgressRepository;
            _lessons = lessonRepository;
            _enrollments = enrollmentRepository;
            _mapper = mapper;
        }
        private async Task EnsureEnrolled(Guid lessonId, Guid studentId)
        {
            var lesson = ServiceChecks.Found(await _lessons.GetAsync(l => l.Id == lessonId), "Lesson");
            if (!await _enrollments.ExistsAsync(e => e.CourseId == lesson.CourseId && e.StudentId == studentId))
                throw new ForbiddenException("Enrollment required.");
        }

        public async Task<double> GetCourseProgressAsync(Guid studentId, Guid courseId)
        {
            var (lessons, total) = await _lessons.GetAllAsync(filter: l => l.CourseId == courseId, tracikng: false);
            if (total == 0) return 0;

            var (done, _) = await _progress.GetAllAsync(filter: p => p.StudentId == studentId && p.Lesson.CourseId == courseId && p.IsCompleted, tracikng: false);
            return (double)done.Count() / total * 100;
        }

        public async Task<IEnumerable<LessonProgressResponseDto>> GetStudentProgressAsync(Guid studentId, Guid courseId)
        {
            var (items, _) = await _progress.GetAllAsync(filter: p => p.StudentId == studentId && p.Lesson.CourseId == courseId, tracikng: false);
            return _mapper.Map<IEnumerable<LessonProgressResponseDto>>(items);
        }

        public async Task<bool> IsLessonCompletedAsync(Guid lessonId, Guid studentId)
            => await _progress.ExistsAsync(p => p.LessonId == lessonId && p.StudentId == studentId && p.IsCompleted);

        public async Task MarkLessonCompletedAsync(Guid lessonId, Guid studentId)
        {
            await EnsureEnrolled(lessonId, studentId);
            var entity = await _progress.GetAsync(p => p.LessonId == lessonId && p.StudentId == studentId);

            if(entity is null)
            {
                entity = new LessonProgress { LessonId = lessonId, StudentId = studentId };
                await _progress.AddAsync(entity);
            }

            entity.IsCompleted = true;
            entity.CompletedAt = DateTime.UtcNow;

            await _progress.SaveAsync();
        }

        public async Task MarkLessonInCompleteAsync(Guid lessonId, Guid studentId)
        {
            await EnsureEnrolled(lessonId, studentId);
            var entity = await _progress.GetAsync(p => p.LessonId == lessonId && p.StudentId == studentId);

            if(entity is null) return;

            entity.IsCompleted = false;
            entity.CompletedAt = null;

            await _progress.SaveAsync();
        }
    }
}
