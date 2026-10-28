using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.CourseCategory
{
    public class UpdateCourseCategoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }
    }
}
