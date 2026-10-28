
namespace OLP.Application.DTOs.CourseCategory
{
    public class CourseCategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
