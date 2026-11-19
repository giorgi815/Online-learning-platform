using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Payment;
using OLP.Application.Exceptions;
using System.Collections.Concurrent;

namespace OLP.Application.Services
{
    public sealed class MockPaymentGateway : IPaymentGateway
    {
        private readonly ConcurrentDictionary<string, GatewayPayment> _sessions = new();

        public Task<GatewayCheckout> CreateAsync(Guid paymentId, decimal amount, string currency)
        {
            var sessionId = Guid.NewGuid().ToString("N");

            _sessions[sessionId] = new GatewayPayment
            {
                SessionId = sessionId,
                PaymentId = paymentId,
                Amount = amount,
                Currency = currency,
                IsPaid = true
            };

            return Task.FromResult(new GatewayCheckout
            {
                SessionId = sessionId,
                CheckoutUrl = $"/mock-checkout/{sessionId}"
            });
        }

        public Task<GatewayPayment> VerifyAsync(string providerSessionId)
        {
            if (!_sessions.TryGetValue(providerSessionId, out var payment))
                throw new BadRequestException("Demo session expired or not found. Create a new checkout.");

            return Task.FromResult(payment);
        }
    }
}
