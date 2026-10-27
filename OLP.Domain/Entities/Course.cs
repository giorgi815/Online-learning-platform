
using OLP.Domain.Enums;

namespace OLP.Domain.Entities
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // FK Teacher
        public Guid TeacherId { get; set; }
        public User Teacher { get; set; } = null!;
        // FK Category
        public Guid CategoryId { get; set; }
        public CourseCategory Category { get; set; } = null!;

        public int Duration { get; set; }

        public decimal Price { get; set; }

        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
