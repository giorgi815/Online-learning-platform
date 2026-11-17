using OLP.Application.Contracts.Persistence;
using OLP.Domain.Entities;
using OLP.Infrastructure.Data;

namespace OLP.Infrastructure.Persistence
{
    public class PaymentRepository : RepositoryBase<Payment, ApplicationDbContext>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
            
        }
    }
}
