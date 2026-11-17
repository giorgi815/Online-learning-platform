using OLP.Application.DTOs.Payment;

namespace OLP.Application.Contracts.Services
{
    public interface IPaymentService
    {
        Task<CheckoutResponseDto> CreateCheckoutAsync(Guid studentId, Guid courseId);
        Task<PaymentResponseDto> ConfirmPaymentAsync(Guid studentId, Guid paymentId);
    }
}
