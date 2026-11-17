using OLP.Application.DTOs.Payment;

namespace OLP.Application.Contracts.Services
{
    public interface IPaymentGateway
    {
        Task<GatewayCheckout> CreateAsync(
        Guid paymentId, decimal amount, string currency);

        Task<GatewayPayment> VerifyAsync(string providerSessionId);
    }
}
