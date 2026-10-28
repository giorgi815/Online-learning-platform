using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Submission
{
    public class GradeSubmissionDto
    {
        [Range(typeof(decimal), "0", "10000")]
        public decimal Score { get; set; }

        [MaxLength(2000)]
        public string? Feedback { get; set; }
    }
}
