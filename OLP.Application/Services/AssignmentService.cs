using MapsterMapper;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Assignment;
using OLP.Domain.Entities;

namespace OLP.Application.Services
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;

        public AssignmentService(IAssignmentRepository assignmentRepository,
            ICourseRepository courseRepository,
            IMapper mapper)
        {
            _assignmentRepository = assignmentRepository;
            _courseRepository = courseRepository;
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
            var assignment = ServiceChecks.Found(_assignmentRepository.GetAsync(a => a.Id == assignmentId).Result, "Assignment");
            await CheckOwner(assignment.CourseId, teacherId);

            _assignmentRepository.Remove(assignment);
            await _assignmentRepository.SaveAsync();
        }

        public async Task<IEnumerable<AssignmentResponseDto>> GetAllAssignmentsAsync()
        {
            var (items, _) = await _assignmentRepository.GetAllAsync(tracikng: false);
            return _mapper.Map<IEnumerable<AssignmentResponseDto>>(items);
        }

        public async Task<AssignmentResponseDto> GetAssignmentByIdAsync(Guid assignmentId)
            => _mapper.Map<AssignmentResponseDto>(ServiceChecks.Found(await _assignmentRepository.GetAsync(a => a.Id == assignmentId, tracking: false), "Assignment"));

        public async Task<IEnumerable<AssignmentResponseDto>> GetAssignmentsByCourseAsync(Guid courseId)
        {
            var (items, _) = await _assignmentRepository.GetAllAsync(filter: a => a.CourseId == courseId, tracikng: false);
            return _mapper.Map<IEnumerable<AssignmentResponseDto>>(items);
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
