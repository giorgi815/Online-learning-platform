using OLP.Application.Contracts.Persistence;
using OLP.Domain.Entities;
using OLP.Infrastructure.Data;

namespace OLP.Infrastructure.Persistence
{
    public class UserRepository : RepositoryBase<User, ApplicationDbContext>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
