
namespace OLP.Application.DTOs.Certificate
{
    public class CertificateResponseDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseTitle { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
    }
}
