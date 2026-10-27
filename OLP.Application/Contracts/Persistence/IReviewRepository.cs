using Microsoft.EntityFrameworkCore;
using OLP.Domain.Entities;

namespace OLP.Application.Contracts.Persistence
{
    public interface IReviewRepository : IRepositoryBase<Review, DbContext>
    {
    }
}
