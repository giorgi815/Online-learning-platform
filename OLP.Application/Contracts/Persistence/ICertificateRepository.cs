using Microsoft.EntityFrameworkCore;
using OLP.Domain.Entities;

namespace OLP.Application.Contracts.Persistence
{
    public interface ICertificateRepository : IRepositoryBase<Certificate, DbContext>
    {
    }
}
