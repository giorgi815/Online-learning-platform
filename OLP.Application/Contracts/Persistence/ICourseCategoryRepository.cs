using Microsoft.EntityFrameworkCore;
using OLP.Domain.Entities;

namespace OLP.Application.Contracts.Persistence
{
    public interface ICourseCategoryRepository : IRepositoryBase<CourseCategory, DbContext>
    {
    }
}
