using MapsterMapper;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Certificate;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;

namespace OLP.Application.Services
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificates;
        private readonly IEnrollmentRepository _enrollments;
        private readonly ILessonRepository _lessons;
        private readonly ILessonProgressRepository _progress;
        private readonly IUserRepository _profiles;
        private readonly ICourseRepository _courses;
        private readonly IMapper _mapper;

        public CertificateService(
            ICertificateRepository certificates,
            IEnrollmentRepository enrolments,
            ILessonRepository lesonns,
            ILessonProgressRepository progress,
            IUserRepository profiles,
            ICourseRepository courses,
            IMapper mapper)
        {
            _certificates = certificates;
            _enrollments = enrolments;
            _lessons = lesonns;
            _progress = progress;
            _profiles = profiles;
            _courses = courses;
            _mapper = mapper;
        }

        public async Task<CertificateResponseDto> GetAsync(Guid studentId, Guid certificateId)
        {
            var certificate = ServiceChecks.Found(await _certificates.GetAsync(
                c => c.Id == certificateId
                && c.StudentId == studentId,
                tracking: false), "Certificate");

            return _mapper.Map<CertificateResponseDto>(certificate);
        }

        public async Task<CertificateResponseDto> IssueAsync(Guid studentId, Guid courseId)
        {
            if (!await _enrollments.ExistsAsync(e => e.StudentId == studentId && e.CourseId == courseId))
                throw new ForbiddenException("Enrollment required.");

            var existing = await _certificates.GetAsync(c => c.StudentId == studentId && c.CourseId == courseId);

            if (existing is not null)
                return _mapper.Map<CertificateResponseDto>(existing);
            
            var (lessons, _) = await _lessons.GetAllAsync(
                filter: l => l.CourseId == courseId, tracikng: false);

            var (completed, _) = await _progress.GetAllAsync(
                filter: p => p.StudentId == studentId
                && p.Lesson.CourseId == courseId
                && p.IsCompleted, tracikng: false);

            var lessonIds = lessons.Select(l => l.Id).ToHashSet();

            var completedIds = completed.Select(p => p.LessonId).ToHashSet();

            if (lessonIds.Count == 0 || !lessonIds.IsSubsetOf(completedIds))
                throw new BadRequestException("Complete all course lessons first.");

            var student = ServiceChecks.Found(await _profiles.GetAsync(u => u.Id == studentId), "Student");
            var course = ServiceChecks.Found(await _courses.GetAsync(c => c.Id == courseId), "Course");
            
            var certificate = new Certificate
            {
                StudentId = studentId,
                CourseId = courseId,
                StudentName = $"{student.FirstName} {student.LastName}",
                CourseTitle = course.Title
            };

            await _certificates.AddAsync(certificate);
            await _certificates.SaveAsync();
            return _mapper.Map<CertificateResponseDto>(certificate);
        }
    }
}
