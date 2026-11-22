using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OLP.Application.Contracts.Services;
using OLP.Security;

namespace OLP.Controllers
{
    [ApiController]
    [Authorize(Roles = "Student")]
    [Route("api/payments")]
    public sealed class PaymentController : ControllerBase
    {
        private readonly IPaymentService _payments;
        private readonly ICurrentUser _current;

        public PaymentController(
            IPaymentService payments,
            ICurrentUser current)
        {
            _payments = payments;
            _current = current;
        }


        [HttpPost("courses/{courseId:guid}/checkout")]
        public async Task<IActionResult> Checkout(Guid courseId)
            => Ok(await _payments.CreateCheckoutAsync(await _current.ProfileIdAsync(), courseId));


        [HttpPost("{paymentId:guid}/confirm")]
        public async Task<IActionResult> Confirm(Guid paymentId)
            => Ok(await _payments.ConfirmPaymentAsync(await _current.ProfileIdAsync(), paymentId));
    }
}
