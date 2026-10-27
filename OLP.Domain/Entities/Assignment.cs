
namespace OLP.Domain.Entities
{
    public class Assignment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime DueDate { get; set; }

        public decimal MaxScore { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK Course
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;

        // Collection Navigation Property
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
