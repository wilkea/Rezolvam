using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rezolvam.Domain.Report.StatusChanges;
using rezolvam.Domain.Reports;

namespace rezolvam.Infrastructure.Persistence.Configurations.Reports
{
    public class StatusChangeConfiguration : IEntityTypeConfiguration<StatusChange>
    {
        public void Configure(EntityTypeBuilder<StatusChange> builder)
        {
            builder.ToTable("ReportStatusChanges");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.ReportId).IsRequired();
            builder.Property(s => s.Status).HasConversion<string>().IsRequired();
            builder.Property(s => s.ChangedBy).IsRequired().HasMaxLength(100);
            builder.Property(s => s.Reason).HasMaxLength(1000);
            builder.Property(s => s.ChangedAt).IsRequired();

            builder.HasOne<Report>()
                .WithMany(r => r.StatusHistory)
                .HasForeignKey(s => s.ReportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
