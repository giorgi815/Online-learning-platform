
using OLP.Application.DTOs.CourseCategory;

namespace OLP.Application.Contracts.Services
{
    public interface ICourseCategoryService
    {
        Task<CourseCategoryResponseDto> CreateCategoryAsync(CreateCourseCategoryDto dto);

        Task<CourseCategoryResponseDto> UpdateCategoryAsync(Guid categoryId, UpdateCourseCategoryDto dto);

        Task DeleteCategoryAsync(Guid categoryId);

        Task<CourseCategoryResponseDto> GetCategoryByIdAsync(Guid categoryId);

        Task<IEnumerable<CourseCategoryResponseDto>> GetAllCategoriesAsync();
    }
}
