using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Submission;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;

namespace OLP.Application.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissions;
        private readonly IAssignmentRepository _assignments;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IFileStorageService _files;
        private readonly IMapper _mapper;

        public SubmissionService(ISubmissionRepository submissionRepository,
            IAssignmentRepository assignmentRepository,
            IFileStorageService files,
            IEnrollmentRepository enrollmentRepository,
            IMapper mapper)
        {
            _submissions = submissionRepository;
            _assignments = assignmentRepository;
            _files = files;
            _enrollments = enrollmentRepository;
            _mapper = mapper;
        }

        public async Task<SubmissionResponseDto> GetSubmissionByIdAsync(Guid submissionId, Guid requestId)
        {
            var submission = ServiceChecks.Found(await _submissions.GetAsync(s => s.Id == submissionId, tracking: false), "Submission");
            var assignment = await FindAssignment(submission.AssignmentId);

            if (submission.StudentId != requestId && assignment.Course.TeacherId != requestId)
                throw new ForbiddenException("Not allowed to view this submission.");

            return _mapper.Map<SubmissionResponseDto>(submission);
        }

        public async Task<IEnumerable<SubmissionResponseDto>> GetSubmissionsByAssignmentAsync(Guid assignmentId, Guid teacherId)
        {
            var assignment = await FindAssignment(assignmentId);
            ServiceChecks.Owner(assignment.Course.TeacherId, teacherId);

            var (items, _) = await _submissions.GetAllAsync(
                filter: s => s.AssignmentId == assignmentId,
                tracikng: false);

            return _mapper.Map<IEnumerable<SubmissionResponseDto>>(items);
        }

        public async Task<IEnumerable<SubmissionResponseDto>> GetSubmissionsByStudentAsync(Guid studentId)
        {
            var (items, _) = await _submissions.GetAllAsync(filter: s => s.StudentId == studentId, tracikng: false);
            return _mapper.Map<IEnumerable<SubmissionResponseDto>>(items);
        }

        public async Task<SubmissionResponseDto> GradeSubmissionAsync(Guid submissionId, GradeSubmissionDto dto, Guid teacherId)
        {
            var enitity = ServiceChecks.Found(await _submissions.GetAsync(s => s.Id == submissionId), "Submission");
            var assignment = await FindAssignment(enitity.AssignmentId);

            ServiceChecks.Owner(assignment.Course.TeacherId, teacherId);
            ServiceChecks.Require(dto.Score >= 0 && dto.Score <= assignment.MaxScore, "Score must be between 0 and the assignment's max score.");

            enitity.Score = dto.Score;
            enitity.Feedback = dto.Feedback;

            await _submissions.SaveAsync();
            return _mapper.Map<SubmissionResponseDto>(enitity);
        }

        public async Task<SubmissionResponseDto> SubmitAssignmentAsync(CreateSubmissionDto dto, Guid studentId)
        {
            var assignment = await FindAssignment(dto.AssignmentId);
            if(!await _enrollments.ExistsAsync(e => e.StudentId == studentId && e.CourseId == assignment.CourseId))
                throw new ForbiddenException("Enrollment required.");

            ServiceChecks.Require(DateTime.UtcNow <= assignment.DueDate, "Assignment deadline has passed.");
            if(await _submissions.ExistsAsync(s => s.AssignmentId == dto.AssignmentId && s.StudentId == studentId))
                throw new ConflictException("Assignment already submitted.");

            var entity = _mapper.Map<Submission>(dto);
            entity.StudentId = studentId;

            await _submissions.AddAsync(entity);
            await _submissions.SaveAsync();

            return _mapper.Map<SubmissionResponseDto>(entity);
        }

        public async Task<SubmissionResponseDto> UpdateSubmissionAsync(Guid submissionId, UpdateSubmissionDto dto, Guid studentId)
        {
            var entity = ServiceChecks.Found(await _submissions.GetAsync(s => s.Id == submissionId), "Submission");
            ServiceChecks.Owner(entity.StudentId, studentId);

            var assignment = await FindAssignment(entity.AssignmentId);
            ServiceChecks.Require(DateTime.UtcNow <= assignment.DueDate && entity.Score is null, "Cannot edit a late or graded submission.");

            _mapper.Map(dto, entity);

            await _submissions.SaveAsync();
            return _mapper.Map<SubmissionResponseDto>(entity);
        }

        public async Task<SubmissionResponseDto> UploadAttachmentAsync(Guid submissionId, Guid studentId, Stream content, string fileName)
        {
            var entity = ServiceChecks.Found(await _submissions.GetAsync(s => s.Id == submissionId), "Submission");

            ServiceChecks.Owner(entity.StudentId, studentId);
            
            var assignment = await FindAssignment(entity.AssignmentId);
            
            if (!await _enrollments.ExistsAsync(e => e.StudentId == studentId && e.CourseId == assignment.CourseId))
                throw new ForbiddenException("Enrollment required.");
            
            ServiceChecks.Require(DateTime.UtcNow <= assignment.DueDate && entity.Score is null,
                "Cannot edit a late or graded submission.");
            
            var key = await _files.UploadAsync(content, fileName);
            entity.Attachments.Add(key);
            await _submissions.SaveAsync();
            
            return _mapper.Map<SubmissionResponseDto>(entity);
        }

        public async Task<Stream> OpenAttachmentAsync(Guid submissionId, Guid actorId, string key)
        {
            var entity = ServiceChecks.Found(await _submissions.GetAsync(s => s.Id == submissionId, tracking: false), "Submission");
            var assignment = await FindAssignment(entity.AssignmentId);

            if (entity.StudentId != actorId && assignment.Course.TeacherId != actorId)
                throw new ForbiddenException("Not allowed to view this submission.");
            
            if (!entity.Attachments.Contains(key))
                throw new NotFoundException("Attachment not found.");
            
            return await _files.OpenReadAsync(key);
        }

        private async Task<Assignment> FindAssignment(Guid id)
        => ServiceChecks.Found(await _assignments.GetAsync(a => a.Id == id, include: q => q.Include(a => a.Course)), "Assignment");
    }
}
