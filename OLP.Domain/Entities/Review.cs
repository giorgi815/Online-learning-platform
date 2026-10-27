
namespace OLP.Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK Student
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        // FK Course
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
