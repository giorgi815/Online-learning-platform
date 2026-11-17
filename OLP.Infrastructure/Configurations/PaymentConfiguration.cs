using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OLP.Domain.Entities;

namespace OLP.Infrastructure.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Student).WithMany().HasForeignKey(x => x.StudentId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(x => x.Course).WithMany().HasForeignKey(x => x.CourseId).OnDelete(DeleteBehavior.NoAction);
            builder.Property(x => x.Amount).HasPrecision(18, 2);
            builder.Property(x => x.Currency).HasMaxLength(3).IsRequired();
            builder.Property(x => x.ProviderSessionId).HasMaxLength(200);
            builder.HasIndex(x => x.ProviderSessionId).IsUnique();
            builder.HasIndex(x => new { x.StudentId, x.CourseId });
        }
    }
}
