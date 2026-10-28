using OLP.Application.Validation;
using System.ComponentModel.DataAnnotations;

namespace OLP.Application.DTOs.Submission
{
    public class CreateSubmissionDto
    {
        [NotEmptyGuid]
        public Guid AssignmentId { get; set; }

        [MaxLength(10000)]
        public string Content { get; set; } = string.Empty;

        public List<string> Attachments { get; set; } = new();
    }
}
