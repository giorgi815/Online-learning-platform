using OLP.Application.Contracts.Persistence;
using OLP.Domain.Entities;
using OLP.Infrastructure.Data;

namespace OLP.Infrastructure.Persistence
{
    public class CourseCategoryRepository : RepositoryBase<CourseCategory, ApplicationDbContext>, ICourseCategoryRepository
    {
        public CourseCategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
