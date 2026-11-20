using MapsterMapper;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.CourseCategory;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;

namespace OLP.Application.Services
{
    public class CourseCategoryService : ICourseCategoryService
    {

        private readonly ICourseCategoryRepository _courseCategoryRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;
        private const string CategoryListKey = "categories:all";
        public CourseCategoryService(
            ICourseCategoryRepository courseCategoryRepository,
            ICourseRepository courseRepository,
            IMapper mapper,
            ICacheService cache)
        {
            _courseCategoryRepository = courseCategoryRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<CourseCategoryResponseDto> CreateCategoryAsync(CreateCourseCategoryDto dto)
        {
            if (dto is null)
                throw new BadRequestException(nameof(dto));

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Category name cannot be empty.");

            if (await _courseCategoryRepository.ExistsAsync(c => c.Name == dto.Name))
                throw new ConflictException($"Category with name {dto.Name} already exists.");

            var category = _mapper.Map<CourseCategory>(dto);

            await _courseCategoryRepository.AddAsync(category);
            await _courseCategoryRepository.SaveAsync();
            await _cache.RemoveAsync(CategoryListKey);

            return _mapper.Map<CourseCategoryResponseDto>(category);
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _courseCategoryRepository.GetAsync(c => c.Id == categoryId);

            if (category is null)
                throw new NotFoundException($"Category with ID {categoryId} not found.");

            if (await _courseRepository.ExistsAsync(c => c.CategoryId == categoryId))
                throw new ConflictException("Cannot delete category with associated courses.");

            _courseCategoryRepository.Remove(category);
            await _courseCategoryRepository.SaveAsync();
            await _cache.RemoveAsync(CategoryListKey);
        }

        public async Task<IEnumerable<CourseCategoryResponseDto>> GetAllCategoriesAsync()
        {
            var cached = await _cache.GetAsync<List<CourseCategoryResponseDto>>(CategoryListKey);
            if (cached is not null) return cached;

            var (items, _) = await _courseCategoryRepository.GetAllAsync();
            var result = _mapper.Map<List<CourseCategoryResponseDto>>(items);

            await _cache.SetAsync(CategoryListKey, result, TimeSpan.FromMinutes(5));
            return result;
        }

        public async Task<CourseCategoryResponseDto> GetCategoryByIdAsync(Guid categoryId)
            => _mapper.Map<CourseCategoryResponseDto>(ServiceChecks.Found(
                await _courseCategoryRepository.GetAsync(c => c.Id == categoryId, tracking: false),
                "Category"));

        public async Task<CourseCategoryResponseDto> UpdateCategoryAsync(Guid categoryId, UpdateCourseCategoryDto dto)
        {
            var category = ServiceChecks.Found(
                await _courseCategoryRepository.GetAsync(c => c.Id == categoryId), "Category");

            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Category name cannot be empty.");

            if (await _courseCategoryRepository.ExistsAsync(x => x.Id != categoryId && x.Name == dto.Name))
                throw new ConflictException($"Category with name {dto.Name} already exists.");

            _mapper.Map(dto, category);

            await _courseCategoryRepository.SaveAsync();
            await _cache.RemoveAsync(CategoryListKey);

            return _mapper.Map<CourseCategoryResponseDto>(category);
        }
    }
}
