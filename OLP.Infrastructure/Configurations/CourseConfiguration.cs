using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OLP.Domain.Entities;

namespace OLP.Infrastructure.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(3000);

            builder.Property(c => c.Price)
                .HasPrecision(18, 2);

            builder.Property(c => c.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Course_Price",
                    "[Price] >= 0");

                t.HasCheckConstraint(
                    "CK_Course_Duration",
                    "[Duration] > 0");

                t.HasCheckConstraint(
                    "CK_Course_Status",
                    "[Status] IN (1, 2)");
            });

            builder.HasIndex(c => c.TeacherId);
            builder.HasIndex(c => c.CategoryId);

            builder.HasOne(c => c.Teacher)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(c => c.Category)
                .WithMany(cc => cc.Courses)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
