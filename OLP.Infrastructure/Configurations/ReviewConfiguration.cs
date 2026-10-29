using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OLP.Domain.Entities;

namespace OLP.Infrastructure.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Comment)
                .HasMaxLength(1000);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Review_Rating",
                    "[Rating] BETWEEN 1 AND 5");
            });

            builder.HasIndex(r => new
            {
                r.StudentId,
                r.CourseId
            }).IsUnique();

            builder.HasOne(r => r.Student)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Course)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
