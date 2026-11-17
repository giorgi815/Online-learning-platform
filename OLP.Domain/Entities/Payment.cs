
namespace OLP.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "USD";
        public string? ProviderSessionId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? PaidAt { get; set; }
    }
}
