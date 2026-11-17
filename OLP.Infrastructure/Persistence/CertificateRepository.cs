using OLP.Application.Contracts.Persistence;
using OLP.Domain.Entities;
using OLP.Infrastructure.Data;

namespace OLP.Infrastructure.Persistence
{
    public class CertificateRepository : RepositoryBase<Certificate, ApplicationDbContext>, ICertificateRepository
    {
        public CertificateRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
