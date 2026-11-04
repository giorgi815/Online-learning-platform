using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.CourseCategory;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;
using Org.BouncyCastle.Asn1.Cms.Ecc;

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
            if(dto is null)
                throw new BadRequestException(nameof(dto));

            if(string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Category name cannot be empty.");

            if(await _courseCategoryRepository.ExistsAsync(c => c.Name == dto.Name))
                throw new ConflictException($"Category with name {dto.Name} already exists.");

            var category = _mapper.Map<CourseCategory>(dto);

            await _courseCategoryRepository.AddAsync(category);
            await _courseCategoryRepository.SaveAsync();

            return _mapper.Map<CourseCategoryResponseDto>(category);
        }

        public async Task DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _courseCategoryRepository.GetAsync(c => c.Id == categoryId);

            if(await _courseRepository.ExistsAsync(c => c.CategoryId == categoryId))
                throw new ConflictException("Cannot delete category with associated courses.");

            if (category is null)
                throw new NotFoundException($"Category with ID {categoryId} not found.");

            _courseCategoryRepository.Remove(category);

            await _courseCategoryRepository.SaveAsync();
        }

        public async Task<IEnumerable<CourseCategoryResponseDto>> GetAllCategoriesAsync()
        {
            var categories = await _courseCategoryRepository.GetAllAsync();

            return _mapper.Map<IEnumerable<CourseCategoryResponseDto>>(categories);
        }

        public async Task<CourseCategoryResponseDto> GetCategoryByIdAsync(Guid categoryId)
            => _mapper.Map<CourseCategoryResponseDto>(await _courseCategoryRepository.GetAsync(c => c.Id == categoryId));

        public async Task<CourseCategoryResponseDto> UpdateCategoryAsync(Guid categoryId, UpdateCourseCategoryDto dto)
        {
            var category = await _courseCategoryRepository.GetAsync(c => c.Id == categoryId);

            if(string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Category name cannot be empty.");

            if(await _courseCategoryRepository.ExistsAsync(x => x.Id != categoryId && x.Name == dto.Name))
                throw new ConflictException($"Category with name {dto.Name} already exists.");

            _mapper.Map(dto, category);

            await _courseCategoryRepository.SaveAsync();

            return _mapper.Map<CourseCategoryResponseDto>(category);
        }
    }
}
