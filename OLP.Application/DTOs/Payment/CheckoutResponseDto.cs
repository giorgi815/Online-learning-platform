
namespace OLP.Application.DTOs.Payment
{
    public class CheckoutResponseDto
    {
        public Guid PaymentId { get; set; }
        public string CheckoutUrl { get; set; } = string.Empty;
    }
}
