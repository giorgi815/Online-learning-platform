using OLP.Application.Contracts.Persistence;
using OLP.Domain.Entities;
using OLP.Infrastructure.Data;

namespace OLP.Infrastructure.Persistence
{
    public class LessonProgressRepository : RepositoryBase<LessonProgress, ApplicationDbContext>, ILessonProgressRepository
    {
        public LessonProgressRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
