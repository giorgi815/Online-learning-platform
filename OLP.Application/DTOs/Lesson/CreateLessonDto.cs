using OLP.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Lesson
{
    public class CreateLessonDto
    {
        [NotEmptyGuid]
        public Guid CourseId { get; set; }

        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(10000, MinimumLength = 10)]
        public string Content { get; set; } = string.Empty;

        public List<string> Attachments { get; set; } = new();

        [Range(0, 10000)]
        public int OrderIndex { get; set; }
    }
}
