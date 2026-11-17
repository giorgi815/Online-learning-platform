
namespace OLP.Domain.Entities
{
    public class Certificate
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public string StudentName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    }
}
