using OLP.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Review
{
    public class CreateReviewDto
    {
        [NotEmptyGuid]
        public Guid CourseId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
