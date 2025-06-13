using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rezolvam.Domain.ReportPhotos;
using rezolvam.Domain.Reports;

namespace rezolvam.Infrastructure.Persistence.Configurations.Reports
{
    public class ReportPhotoConfiguration : IEntityTypeConfiguration<ReportPhoto>
    {
        public void Configure(EntityTypeBuilder<ReportPhoto> builder)
        {
            builder.ToTable("ReportPhotos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.PhotoUrl).IsRequired().HasMaxLength(4000);
            builder.Property(p => p.FileName).HasMaxLength(255);
            builder.Property(p => p.FileSize);
            builder.Property(p => p.UploadedAt).IsRequired();
            builder.Property(p => p.ReportId).IsRequired();

            builder.HasOne<Report>()
                .WithMany(r => r.Photos)
                .HasForeignKey(p => p.ReportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
