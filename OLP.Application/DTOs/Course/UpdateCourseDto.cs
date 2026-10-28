using OLP.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Course
{
    public class UpdateCourseDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(3000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [NotEmptyGuid]
        public Guid CategoryId { get; set; }

        [Range(1, 10000)]
        public int Duration { get; set; }

        [Range(typeof(decimal), "0", "100000")]
        public decimal Price { get; set; }
    }
}
