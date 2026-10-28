using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Submission
{
    public class UpdateSubmissionDto
    {
        [MaxLength(10000)]
        public string Content { get; set; } = string.Empty;
        public List<string> Attachments { get; set; } = new();
    }
}
