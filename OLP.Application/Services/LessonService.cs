using MapsterMapper;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Lesson;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;
using OLP.Domain.Enums;

namespace OLP.Application.Services
{
    public class LessonService : ILessonService
    {
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IFileStorageService _files;
        private readonly IEnrollmentRepository _enrollments;
        private readonly IMapper _mapper;
        public LessonService(
            ILessonRepository lessonRepository,
            ICourseRepository courseRepository,
            IFileStorageService files,
            IEnrollmentRepository enrollemnts,
            IMapper mapper)
        {
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
            _files = files;
            _enrollments = enrollemnts;
            _mapper = mapper;
        }

        private async Task CheckOwner(Guid courseId, Guid teacherId)
        {
            var course = ServiceChecks.Found(await _courseRepository.GetAsync(c => c.Id == courseId), "Course");
            ServiceChecks.Owner(course.TeacherId, teacherId);
        }

        public async Task<LessonResponseDto> CreateLessonAsync(CreateLessonDto dto, Guid teacherId)
        {
            await CheckOwner(dto.CourseId, teacherId);

            if(await _lessonRepository.ExistsAsync(l => l.CourseId == dto.CourseId && l.OrderIndex == dto.OrderIndex))
                throw new ConflictException("Lesson order is already used.");

            var lesson = _mapper.Map<Lesson>(dto);
            await _lessonRepository.AddAsync(lesson);
            await _lessonRepository.SaveAsync();
            return _mapper.Map<LessonResponseDto>(lesson);
        }

        public async Task DeleteLessonAsync(Guid lessonId, Guid teacherId)
        {
            var lesson = ServiceChecks.Found(_lessonRepository.GetAsync(l => l.Id == lessonId).Result, "Lesson");
            
            await CheckOwner(lesson.CourseId, teacherId);
            _lessonRepository.Remove(lesson);

            await _lessonRepository.SaveAsync();
        }

        public async Task<IEnumerable<LessonResponseDto>> GetAllLessonsAsync()
        {
            var (items, _) = await _lessonRepository.GetAllAsync(tracikng: false);
            return _mapper.Map<IEnumerable<LessonResponseDto>>(items);
        }

        public async Task<IEnumerable<LessonResponseDto>> GetLessonByCourseAsync(Guid courseId, Guid actorId)
        {
            await CheckAccess(courseId, actorId);
            var (items, _) = await _lessonRepository.GetAllAsync(
                filter: l => l.CourseId == courseId,
                orderBy: l => l.OrderIndex,
                tracikng: false);

            return _mapper.Map<IEnumerable<LessonResponseDto>>(items);
        }

        public async Task<LessonResponseDto> GetLessonByIdAsync(Guid lessonId, Guid actorId)
        {
            var lesson = ServiceChecks.Found(await _lessonRepository.GetAsync(l => l.Id == lessonId, tracking: false), "Lesson");

            await CheckAccess(lesson.CourseId, actorId);
            return _mapper.Map<LessonResponseDto>(lesson);
        }

        public async Task<LessonResponseDto> UpdateLessonAsync(Guid lessonId, UpdateLessonDto dto, Guid teacherId)
        {
            var lesson = ServiceChecks.Found(await _lessonRepository.GetAsync(l => l.Id == lessonId), "Lesson");
            await CheckOwner(lesson.CourseId, teacherId);

            if(await _lessonRepository.ExistsAsync(l => l.CourseId == lesson.CourseId && l.Id != lessonId && l.OrderIndex == dto.OrderIndex))
                throw new ConflictException("Lesson order is already used.");

            _mapper.Map(dto, lesson);
            await _lessonRepository.SaveAsync();
            return _mapper.Map<LessonResponseDto>(lesson);
        }

        public async Task<LessonResponseDto> UploadAttachmentAsync(Guid lessonId, Guid teacherId, Stream content, string fileName)
        {
            var lesson = ServiceChecks.Found(await _lessonRepository.GetAsync(l => l.Id == lessonId), "Lesson");
            
            await CheckOwner(lesson.CourseId, teacherId);
            
            var key = await _files.UploadAsync(content, fileName);
            
            lesson.Attachments.Add(key);
            await _lessonRepository.SaveAsync();
            return _mapper.Map<LessonResponseDto>(lesson);
        }

        public async Task<Stream> OpenAttachmentAsync(Guid lessonId, Guid actorId, string key)
        {
            var lesson = ServiceChecks.Found(await _lessonRepository.GetAsync(l => l.Id == lessonId, tracking: false), "Lesson");
            var course = ServiceChecks.Found(await _courseRepository.GetAsync(c => c.Id == lesson.CourseId, tracking: false), "Course");
            
            if (course.TeacherId != actorId && (course.Status != CourseStatus.Published ||
                !await _enrollments.ExistsAsync(e => e.CourseId == course.Id && e.StudentId == actorId)))
                throw new ForbiddenException("Course access required.");
            
            if (!lesson.Attachments.Contains(key))
                throw new NotFoundException("Attachment not found.");
            
            return await _files.OpenReadAsync(key);
        }

        private async Task CheckAccess(Guid courseId, Guid actorId)
        {
            var course = ServiceChecks.Found(await _courseRepository.GetAsync(c => c.Id == courseId, tracking: false), "Course");

            if (course.TeacherId != actorId && (course.Status != CourseStatus.Published ||
                !await _enrollments.ExistsAsync(e => e.CourseId == courseId && e.StudentId == actorId)))
                throw new ForbiddenException("Course access required.");
        }
    }
}
