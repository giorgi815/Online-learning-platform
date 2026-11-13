using MapsterMapper;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Submission;

namespace OLP.Application.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissions;
        private readonly IAssignmentRepository _assignments;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IMapper _mapper;

        public SubmissionService(ISubmissionRepository submissionRepository,
            IAssignmentRepository assignmentRepository,
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper)
        {
            _submissions = submissionRepository;
            _assignments = assignmentRepository;
            _enrollments = enrollmentRepository;
            _mapper = mapper;
        }

        public Task<SubmissionResponseDto> GetSubmissionByIdAsync(Guid submissionId, Guid requestId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SubmissionResponseDto>> GetSubmissionsByAssignmentAsync(Guid assignmentId, Guid teacherId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<SubmissionResponseDto>> GetSubmissionsByStudentAsync(Guid studentId)
        {
            throw new NotImplementedException();
        }

        public Task<SubmissionResponseDto> GradeSubmissionAsync(Guid submissionId, GradeSubmissionDto dto, Guid teacherId)
        {
            throw new NotImplementedException();
        }

        public Task<SubmissionResponseDto> SubmitAssignmentAsync(CreateSubmissionDto dto, Guid studentId)
        {
            throw new NotImplementedException();
        }

        public Task<SubmissionResponseDto> UpdateSubmissionAsync(Guid submissionId, UpdateSubmissionDto dto, Guid studentId)
        {
            throw new NotImplementedException();
        }
    }
}
