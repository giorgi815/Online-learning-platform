
namespace OLP.Application.DTOs.LessonProgress
{
    public class LessonProgressResponseDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid LessonId { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
