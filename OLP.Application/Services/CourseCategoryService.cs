using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.CourseCategory;

namespace OLP.Application.Services
{
    public class CourseCategoryService : ICourseCategoryService
    {

        private readonly ICourseCategoryRepository _courseCategoryRepository;


        public Task<CourseCategoryResponseDto> CreateCategoryAsync(CreateCourseCategoryDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCategoryAsync(Guid categoryId)
        {
            throw new NotImplementedException();
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
