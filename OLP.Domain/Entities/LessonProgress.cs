
namespace OLP.Domain.Entities
{
    public class LessonProgress
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        //Fk Student
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        // FK Lesson
        public Guid LessonId { get; set; }
        public Lesson Lesson { get; set; } = null!;
    }
}
