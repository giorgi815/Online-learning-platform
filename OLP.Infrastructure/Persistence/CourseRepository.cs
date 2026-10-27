using OLP.Application.Contracts.Persistence;
using OLP.Domain.Entities;
using OLP.Infrastructure.Data;

namespace OLP.Infrastructure.Persistence
{
    public class CourseRepository : RepositoryBase<Course, ApplicationDbContext>, ICourseRepository
    {
        public CourseRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
