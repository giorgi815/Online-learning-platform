
namespace OLP.Application.DTOs.Payment
{
    public class GatewayPayment
    {
        public string SessionId { get; set; } = string.Empty;
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
    }
}
