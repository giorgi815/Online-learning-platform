using Microsoft.AspNetCore.Identity;
using OLP.Application.Contracts.Persistence;
using OLP.Application.Contracts.Services;
using OLP.Application.DTOs.Payment;
using OLP.Application.Exceptions;
using OLP.Domain.Entities;
using OLP.Domain.Enums;

namespace OLP.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _payments;
        private readonly ICourseRepository _courses;
        private readonly IUserRepository _profiles;
        private readonly IEnrollmentRepository _enrollments;
        private readonly UserManager<ApplicationUser> _users;
        private readonly IPaymentGateway _gateway;

        public PaymentService(IPaymentRepository payments,
            ICourseRepository courses,
            IUserRepository profiles,
            IEnrollmentRepository enrollments,
            UserManager<ApplicationUser> users,
            IPaymentGateway gateway)
        {
            _payments = payments;
            _courses = courses;
            _profiles = profiles;
            _enrollments = enrollments;
            _users = users;
            _gateway = gateway;
        }

        public async Task<CheckoutResponseDto> CreateCheckoutAsync(Guid studentId, Guid courseId)
        {
            await EnsureStudent(studentId);

            var course = ServiceChecks.Found(await _courses.GetAsync(c => c.Id == courseId, tracking: false), "Course");
            ServiceChecks.Require(course.Status == CourseStatus.Published && course.Price > 0, "Published paid course required.");

            if (await _enrollments.ExistsAsync(e => e.StudentId == studentId && e.CourseId == courseId))
                throw new ConflictException("Already enrolled.");
            if (await _payments.ExistsAsync(p => p.StudentId == studentId && p.CourseId == courseId && p.PaidAt != null))
                throw new ConflictException("Course already purchased.");

            var payment = new Payment
            {
                StudentId = studentId,
                CourseId = courseId,
                Amount = course.Price
            };

            await _payments.AddAsync(payment);
            await _payments.SaveAsync();

            var checkout = await _gateway.CreateAsync(payment.Id, payment.Amount, payment.Currency);
            payment.ProviderSessionId = checkout.SessionId;
            await _payments.SaveAsync();

            return new CheckoutResponseDto
            {
                PaymentId = payment.Id,
                CheckoutUrl = checkout.CheckoutUrl
            };
        }

        public async Task<PaymentResponseDto> ConfirmPaymentAsync(Guid studentId, Guid paymentId)
        {
            await EnsureStudent(studentId);
            var payment = ServiceChecks.Found(await _payments.GetAsync(p => p.Id == paymentId && p.StudentId == studentId), "Payment");

            if (payment.PaidAt is not null)
                return Response(payment);

            var session = payment.ProviderSessionId ?? throw new BadRequestException("Checkout not initialized.");

            var verified = await _gateway.VerifyAsync(session);

            if (!verified.IsPaid || verified.SessionId != session || verified.PaymentId != payment.Id ||
                verified.Amount != payment.Amount || !string.Equals(verified.Currency, payment.Currency, StringComparison.OrdinalIgnoreCase))
                throw new BadRequestException("Payment could not be verified");

            payment.PaidAt = DateTime.UtcNow;
            await _payments.SaveAsync();
            return Response(payment);
        }

        private async Task EnsureStudent(Guid studentId)
        {
            var profile = ServiceChecks.Found(await _profiles.GetAsync(u => u.Id == studentId, tracking: false), "Student profile");

            var account = await _users.FindByIdAsync(profile.ApplicationUserId.ToString());

            if (account is null || !await _users.IsInRoleAsync(account, "Student"))
                throw new ForbiddenException("Student role required.");
        }

        private static PaymentResponseDto Response(Payment p)
            => new PaymentResponseDto
            {
                PaymentId = p.Id,
                CourseId = p.CourseId,
                Amount = p.Amount,
                Currency = p.Currency,
                PaidAt = p.PaidAt
            };
    }
}
