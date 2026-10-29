using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OLP.Domain.Entities;

namespace OLP.Infrastructure.Configurations
{
    public class SubmissionConfiguration : IEntityTypeConfiguration<Submission>
    {
        public void Configure(EntityTypeBuilder<Submission> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Content)
                .HasMaxLength(10000);

            builder.Property(s => s.Feedback)
                .HasMaxLength(2000);

            builder.Property(s => s.Score)
                .HasPrecision(18, 2);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Submission_Score",
                    "[Score] IS NULL OR [Score] >= 0");
            });

            builder.HasIndex(s => new
            {
                s.AssignmentId,
                s.StudentId
            }).IsUnique();

            builder.HasOne(s => s.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(s => s.AssignmentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(s => s.Student)
                .WithMany(u => u.Submissions)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
