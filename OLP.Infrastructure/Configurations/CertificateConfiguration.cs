using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OLP.Domain.Entities;

namespace OLP.Infrastructure.Configurations
{
    public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
    {
        public void Configure(EntityTypeBuilder<Certificate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.NoAction);
            builder.HasIndex(x => new { x.StudentId, x.CourseId }).IsUnique();
            builder.Property(x => x.StudentName).HasMaxLength(201).IsRequired();
            builder.Property(x => x.CourseTitle).HasMaxLength(150).IsRequired();
        }
    }
}
