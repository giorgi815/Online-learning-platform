
using OLP.Domain.Enums;

namespace OLP.Application.DTOs.Course
{
    public class CourseResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public CourseStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
