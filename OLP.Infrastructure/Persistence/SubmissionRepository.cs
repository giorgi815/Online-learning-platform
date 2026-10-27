using OLP.Application.Contracts.Persistence;
using OLP.Domain.Entities;
using OLP.Infrastructure.Data;

namespace OLP.Infrastructure.Persistence
{
    public class SubmissionRepository : RepositoryBase<Submission, ApplicationDbContext>, ISubmissionRepository
    {
        public SubmissionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
