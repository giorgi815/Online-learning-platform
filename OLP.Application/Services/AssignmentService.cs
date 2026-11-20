using MapsterMapper;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Assignment;
using OLP.Application.Exceptions;
using OLP.Domain.Enums;
using OLP.Domain.Entities;

namespace OLP.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IMapper _mapper;

        public AssignmentService(IAssignmentRepository assignmentRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollments,
            IMapper mapper)
        {
            _assignmentRepository = assignmentRepository;
            _courseRepository = courseRepository;
            _enrollments = enrollments;
            _mapper = mapper;
        }

        private async Task CheckOwner(Guid courseId, Guid teacherId)
        {
            var course = ServiceChecks.Found(await _courseRepository.GetAsync(c => c.Id == courseId), "Course");
            ServiceChecks.Owner(course.TeacherId, teacherId);
        }

        public async Task<AssignmentResponseDto> CreateAssignmentAsync(CreateAssignmentDto dto, Guid teacherId)
        {
            await CheckOwner(dto.CourseId, teacherId);
            ServiceChecks.Require(dto.MaxScore > 0 && dto.DueDate > DateTime.UtcNow, "Invalid due date or maximum score.");
            var Assignment = _mapper.Map<Assignment>(dto);
            await _assignmentRepository.AddAsync(Assignment);
            await _assignmentRepository.SaveAsync();
            return _mapper.Map<AssignmentResponseDto>(Assignment);
        }

        public async Task DeleteAssignmentAsync(Guid assignmentId, Guid teacherId)
        {
            var assignment = ServiceChecks.Found(await _assignmentRepository.GetAsync(a => a.Id == assignmentId), "Assignment");
            await CheckOwner(assignment.CourseId, teacherId);

            _assignmentRepository.Remove(assignment);
            await _assignmentRepository.SaveAsync();
        }

        public async Task<AssignmentResponseDto> GetAssignmentByIdAsync(Guid assignmentId, Guid actorId)
        {
            var assignment = ServiceChecks.Found(await _assignmentRepository.GetAsync(
                a => a.Id == assignmentId, tracking: false), "Assignment");
            await CheckAccess(assignment.CourseId, actorId);
            return _mapper.Map<AssignmentResponseDto>(assignment);
        }

        public async Task<IEnumerable<AssignmentResponseDto>> GetAssignmentsByCourseAsync(Guid courseId, Guid actorId)
        {
            await CheckAccess(courseId, actorId);
            var (items, _) = await _assignmentRepository.GetAllAsync(
                filter: a => a.CourseId == courseId, tracikng: false);
            return _mapper.Map<IEnumerable<AssignmentResponseDto>>(items);
        }

        private async Task CheckAccess(Guid courseId, Guid actorId)
        {
            var course = ServiceChecks.Found(await _courseRepository.GetAsync(
                c => c.Id == courseId, tracking: false), "Course");
            if (course.TeacherId != actorId &&
                (course.Status != CourseStatus.Published ||
                 !await _enrollments.ExistsAsync(e => e.CourseId == courseId && e.StudentId == actorId)))
                throw new ForbiddenException("Course enrollment or ownership required.");
        }

        public async Task<AssignmentResponseDto> UpdateAssignmentAsync(Guid assignmentId, UpdateAssignmentDto dto, Guid teacherId)
        {
            var assignment = ServiceChecks.Found(await _assignmentRepository.GetAsync(a => a.Id == assignmentId), "Assignment");
            await CheckOwner(assignment.CourseId, teacherId);

            ServiceChecks.Require(dto.MaxScore > 0, "Maximum score must be positive.");
            _mapper.Map(dto, assignment);
            await _assignmentRepository.SaveAsync();
            return _mapper.Map<AssignmentResponseDto>(assignment);
        }
    }
}
