using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rezolvam.Domain.Reports;
using rezolvam.Domain.Report.StatusChanges;
using rezolvam.Domain.ReportComments;
using rezolvam.Domain.ReportPhotos;

namespace rezolvam.Infrastructure.Persistence.Configurations.Reports
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.SubmitedById).IsRequired();

            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(r => r.Location)
                .IsRequired()
                .HasMaxLength(4000);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(r => r.CreatedAt).IsRequired();
            builder.Property(r => r.UpdatedAt);

            // Comments configuration
            builder.HasMany<ReportComment>("_comments")
                .WithOne()
                .HasForeignKey("ReportId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation("_comments")
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Photos configuration
            builder.HasMany<ReportPhoto>("_photos")
                .WithOne()
                .HasForeignKey("ReportId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation("_photos")
                .UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);

            // Status History configuration
            builder.HasMany<StatusChange>("_statusHistory")
                .WithOne()
                .HasForeignKey("ReportId")
                .OnDelete(DeleteBehavior.Cascade);
            builder.Navigation("_statusHistory")
                .UsePropertyAccessMode(PropertyAccessMode.PreferFieldDuringConstruction);
        }
    }
}
