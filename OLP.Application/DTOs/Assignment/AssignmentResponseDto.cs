
namespace OLP.Application.DTOs.Assignment
{
    public class AssignmentResponseDto
    {
        public Guid Id { get; set; }
        public Guid CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public decimal MaxScore { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
