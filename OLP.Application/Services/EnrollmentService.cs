using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Enrollment;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;
using OLP.Domain.Enums;

namespace OLP.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollments;
        private readonly ICourseRepository _courses;
        private readonly IUserRepository _profiles;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPaymentRepository _payments;
        private readonly IMapper _mapper;

        public EnrollmentService(IEnrollmentRepository enrollmentService,
            ICourseRepository courseRepository,
            IUserRepository userRepository,
            IPaymentRepository payments,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _enrollments = enrollmentService;
            _courses = courseRepository;
            _profiles = userRepository;
            _payments = payments;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<EnrollmentResponseDto> EnrollStudentAsync(Guid courseId, Guid studentId)
        {
            var profile = ServiceChecks.Found(await _profiles.GetAsync(u => u.Id == studentId), "Student profile");
            var account = await _userManager.FindByIdAsync(profile.ApplicationUserId.ToString());

            if (account is null || !await _userManager.IsInRoleAsync(account, "Student"))
                throw new ForbiddenException("Student role required.");

            var course = ServiceChecks.Found(await _courses.GetAsync(c => c.Id == courseId), "Course");
            ServiceChecks.Require(course.Status == CourseStatus.Published, "Course is not published.");
            
            if (await _enrollments.ExistsAsync(e => e.StudentId == studentId && e.CourseId == courseId))
                throw new ConflictException("Already enrolled.");
            
            if (course.Price > 0 && !await _payments.ExistsAsync(p =>
                    p.CourseId == courseId && p.StudentId == studentId && p.PaidAt != null))
            {
                throw new ForbiddenException("Purchase this course before enrolling.");
            }
            
            var entity = new Enrollment { CourseId = courseId, StudentId = studentId };
            
            await _enrollments.AddAsync(entity);
            await _enrollments.SaveAsync();
            
            entity.Course = course;
            return _mapper.Map<EnrollmentResponseDto>(entity);
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetEnrollmentsByStudentAsync(Guid studentId)
        {
            var (items, _) = await _enrollments.GetAllAsync(
                filter: e => e.StudentId == studentId,
                tracikng: false,
                includes: [e => e.Course]);

            return _mapper.Map<IEnumerable<EnrollmentResponseDto>>(items);
        }

        public async Task<IEnumerable<EnrollmentResponseDto>> GetStudentsByCourseAsync(Guid courseId, Guid requestId)
        {
            var course = ServiceChecks.Found(await _courses.GetAsync(c => c.Id == courseId), "Course");
            ServiceChecks.Owner(course.TeacherId, requestId);

            var (items, _) = await _enrollments.GetAllAsync(
                filter: e => e.CourseId == courseId,
                tracikng: false,
                includes: [e => e.Course]);

            return _mapper.Map<IEnumerable<EnrollmentResponseDto>>(items);
        }

        public async Task<bool> IsStudentEnrolledAsync(Guid courseId, Guid studentId)
            => await _enrollments.ExistsAsync(e => e.CourseId == courseId && e.StudentId == studentId);

        public async Task UnEnrollStudentAsync(Guid courseId, Guid studentId)
        {
            var enrollment = ServiceChecks.Found(await _enrollments.GetAsync(e => e.CourseId == courseId && e.StudentId == studentId), "Enrollment");

            _enrollments.Remove(enrollment);
            await _enrollments.SaveAsync();
        }
    }
}
