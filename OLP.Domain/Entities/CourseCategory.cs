
namespace OLP.Domain.Entities
{
    public class CourseCategory
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
