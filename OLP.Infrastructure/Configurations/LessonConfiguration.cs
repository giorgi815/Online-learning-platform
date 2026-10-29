using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OLP.Domain.Entities;

namespace OLP.Infrastructure.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            builder.HasKey(l => l.Id);

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(l => l.Content)
                .IsRequired()
                .HasMaxLength(10000);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "CK_Lesson_OrderIndex",
                    "[OrderIndex] >= 0");
            });

            builder.HasIndex(l => new
            {
                l.CourseId,
                l.OrderIndex
            }).IsUnique();

            builder.HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
