using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Course;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;
using OLP.Domain.Enums;

namespace OLP.Application.Services
{
    public class CourseService : ICourseService
    {

        private readonly ICourseRepository _courseRepository;
        private readonly ICourseCategoryRepository _CategoryRepository;
        private readonly IUserRepository _profiles;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public CourseService(ICourseRepository courseRepository,
            ICourseCategoryRepository categoryRepository,
            IUserRepository profiles,
            UserManager<ApplicationUser> userManager,
            IMapper mapper)
        {
            _courseRepository = courseRepository;
            _CategoryRepository = categoryRepository;
            _profiles = profiles;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<CourseResponseDto> CreateCourseAsync(CreateCourseDto dto, Guid teacherId)
        {
            await ValidateTeacher(teacherId);
            if (!await _CategoryRepository.ExistsAsync(x => x.Id == dto.CategoryId))
                throw new NotFoundException("Category not found.");

            ServiceChecks.Require(dto.Price >= 0 && dto.Duration > 0, "Invalid course price or duration.");
            var course = _mapper.Map<Course>(dto);
            course.TeacherId = teacherId;
            course.Status = CourseStatus.Draft;
            await _courseRepository.AddAsync(course);
            await _courseRepository.SaveAsync();
            return await Response(course.Id);
        }

        public async Task DeleteCourseAsync(Guid courseId, Guid teacherId)
        {
            await ValidateTeacher(teacherId);
            var course = ServiceChecks.Found(await _courseRepository.GetAsync(x => x.Id == courseId), "Course");

            if (course.TeacherId != teacherId)
                throw new ForbiddenException("You do not own this resource.");

            _courseRepository.Remove(course);
            await _courseRepository.SaveAsync();

        }

        public async Task<IEnumerable<CourseResponseDto>> GetAllCourseAsync() => await List(x => x.Status == CourseStatus.Published);

        public async Task<IEnumerable<CourseResponseDto>> GetCourseByCategoryAsync(Guid categoryId) => await List(x => x.CategoryId == categoryId && x.Status == CourseStatus.Published);

        public async Task<CourseResponseDto> GetCourseByIdAsync(Guid courseId)
        {
            var course = ServiceChecks.Found(await _courseRepository.GetAsync(
                c => c.Id == courseId && c.Status == CourseStatus.Published,
                tracking: false), "Published course");
            return await Response(course.Id);
        }

        public async Task<IEnumerable<CourseResponseDto>> GetCourseByTeacherIdAsync(Guid teacherId, Guid actorId)
        {
            return await List(x => x.TeacherId == teacherId &&
                (x.Status == CourseStatus.Published || teacherId == actorId));
        }

        public async Task PublishCourse(Guid courseId, Guid teacherId)
        {
            await ValidateTeacher(teacherId);
            var course = ServiceChecks.Found(await _courseRepository.GetAsync(x => x.Id == courseId), "Course");

            if (course.TeacherId != teacherId)
                throw new ForbiddenException("You do not own this resource.");

            course.Status = CourseStatus.Published;
            await _courseRepository.SaveAsync();
        }

        public async Task<CourseResponseDto> UpdateCourseAsync(UpdateCourseDto dto, Guid courseId, Guid teacherId)
        {
            await ValidateTeacher(teacherId);
            var course = ServiceChecks.Found(await _courseRepository.GetAsync(x => x.Id == courseId), "Course");

            if (course.TeacherId != teacherId)
                throw new ForbiddenException("You do not own this resource.");

            if (dto.Price < 0 || dto.Duration <= 0)
                throw new BadRequestException("Invalid course price or duration.");

            if (!await _CategoryRepository.ExistsAsync(x => x.Id == dto.CategoryId))
                throw new NotFoundException("Category not found.");
            _mapper.Map(dto, course);
            await _courseRepository.SaveAsync();
            return await Response(courseId);
        }

        private async Task<CourseResponseDto> Response(Guid id)
        {
            var course = await _courseRepository.GetAsync(
                x => x.Id == id,
                tracking: false,
                include: q => q
                .Include(c => c.Teacher)
                .Include(c => c.Category));

            return _mapper.Map<CourseResponseDto>(ServiceChecks.Found(course, "Course"));
        }

        private async Task ValidateTeacher(Guid teacherId)
        {
            var profile = ServiceChecks.Found(await _profiles.GetAsync(x => x.Id == teacherId), "Teacher profile");

            var account = await _userManager.FindByIdAsync(profile.ApplicationUserId.ToString());

            if (account is null || !await _userManager.IsInRoleAsync(account, "Teacher"))
                throw new ForbiddenException("Teacher role required.");
        }

        private async Task<IEnumerable<CourseResponseDto>> List(System.Linq.Expressions.Expression<Func<Course, bool>> predicate)
        {
            var (items, _) = await _courseRepository.GetAllAsync(
                filter: predicate,
                tracikng: false,
                includes: [c => c.Teacher, c => c.Category]);

            return _mapper.Map<List<CourseResponseDto>>(items.ToList());
        }
    }
}
