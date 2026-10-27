
namespace OLP.Domain.Entities
{
    public class Submission
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Content { get; set; } = string.Empty;

        public List<string> Attachments { get; set; } = new();

        public decimal? Score { get; set; }

        public string? Feedback { get; set; }

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

        // Fk Assignment
        public Guid AssignmentId { get; set; }
        public Assignment Assignment { get; set; } = null!;

        // FK Student
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;
    }
}
