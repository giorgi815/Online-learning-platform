
namespace OLP.Application.DTOs.Submission
{
    public class SubmissionResponseDto
    {
        public Guid Id { get; set; }
        public Guid AssignmentId { get; set; }
        public Guid StudentId { get; set; }
        public string Content { get; set; } = string.Empty;
        public List<string> Attachments { get; set; } = new();
        public decimal? Score { get; set; }
        public string? Feedback { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
