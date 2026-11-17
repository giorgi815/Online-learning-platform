
namespace OLP.Application.DTOs.Payment
{
    public class GatewayCheckout
    {
        public string SessionId { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
    }
}
