
namespace OLP.Application.DTOs.Lesson
{
    public class LessonResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public List<string> Attachments { get; set; } = new();
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
