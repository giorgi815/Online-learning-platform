
namespace OLP.Domain.Entities
{
    public class Enrollment
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        // FK Student
        public Guid StudentId { get; set; }
        public User Student { get; set; } = null!;

        // FK Course
        public Guid CourseId { get; set; }
        public Course Course { get; set; } = null!;
    }
}
