using MapsterMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        private readonly UserManager<ApplicationUser> _userManager;
        


        public async Task<CourseCategoryResponseDto> CreateCategoryAsync(CreateCourseCategoryDto dto)
        {
            if(dto is null)
                throw new BadRequestException(nameof(dto));

            if(string.IsNullOrWhiteSpace(dto.Name))
                throw new BadRequestException("Category name cannot be empty.");

            if(string.IsNullOrWhiteSpace(dto.Description))
                throw new BadRequestException("Category description cannot be empty.");

            var category = _mapper.Map<CourseCategory>(dto);

            await _courseCategoryRepository.AddAsync(category);
            await _courseCategoryRepository.SaveAsync();

            return _mapper.Map<CourseCategoryResponseDto>(category);
        }

        public Task DeleteCategoryAsync(Guid categoryId)
        {
            var category = _courseCategoryRepository.GetAsync(c => c.Id == categoryId).Result;

            if(category is null)
                throw new NotFoundException($"Category with ID {categoryId} not found.");

            _courseCategoryRepository.Delete(category);
        }

        public Task<IEnumerable<CourseCategoryResponseDto>> GetAllCategoriesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<CourseCategoryResponseDto> GetCategoryByIdAsync(Guid categoryId)
        {
            throw new NotImplementedException();
        }

        public Task<CourseCategoryResponseDto> UpdateCategoryAsync(Guid categoryId, UpdateCourseCategoryDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
