using OLP.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Assignment
{
    public class CreateAssignmentDto
    {
        [NotEmptyGuid]
        public Guid CourseId { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(3000, MinimumLength = 10)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime DueDate { get; set; }

        [Range(typeof(decimal), "0.01", "10000")]
        public decimal MaxScore { get; set; }
    }
}
