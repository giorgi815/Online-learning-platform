
namespace OLP.Application.DTOs.Payment
{
    public class PaymentResponseDto
    {
        public Guid PaymentId { get; set; }
        public Guid CourseId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime? PaidAt { get; set; }
    }
}
