using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OLP.Domain.Entities;

namespace OLP.Infrastructure.Configurations
{
    public class AssignmentConfiguration : IEntityTypeConfiguration<Assignment>
    {
        public void Configure(EntityTypeBuilder<Assignment> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(3000);

            builder.Property(a => a.MaxScore)
                .HasPrecision(18, 2);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Assignment_MaxScore",
                    "[MaxScore] > 0");
            });

            builder.HasIndex(a => a.CourseId);

            builder.HasOne(a => a.Course)
                .WithMany(c => c.Assignments)
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
