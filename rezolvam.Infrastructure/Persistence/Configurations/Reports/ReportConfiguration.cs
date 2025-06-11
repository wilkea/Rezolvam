using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rezolvam.Domain.Reports;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rezolvam.Domain.Report.StatusChanges;

namespace rezolvam.Infrastructure.Persistence.Configurations.Reports
{
    public class ReportConfiguration : IEntityTypeConfiguration<Report>
    {
        public void Configure(EntityTypeBuilder<Report> builder)
        {
            builder.ToTable("Reports");

            builder.HasKey(r => r.Id);
            builder.Property(r => r.SubmitedById)
                    .IsRequired();

            builder.Property(r => r.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasConversion<string>();
            builder.Property(r => r.CreatedAt)
                .IsRequired();

            builder.OwnsMany(r => r.Photos, photo =>
            {
                photo.WithOwner().HasForeignKey("ReportId");
                photo.HasKey(p => p.Id); // Explicit key configuration
                photo.Property(p => p.PhotoUrl).IsRequired().HasMaxLength(1000);
                photo.Property(p => p.FileName).HasMaxLength(255);
                photo.Property(p => p.FileSize);
                photo.Property(p => p.UploadedAt).IsRequired();
                photo.ToTable("ReportPhotos"); // Separate table for photos
            });
            builder.OwnsMany(r => r.Comments, comment =>
            {
                comment.WithOwner().HasForeignKey("ReportId");
                comment.HasKey(c => c.Id); // Explicit key configuration
                comment.Property(c => c.AuthorId).IsRequired();
                comment.Property(c => c.Message).IsRequired().HasMaxLength(2000);
                comment.Property(c => c.Type).HasConversion<string>().IsRequired();
                comment.Property(c => c.CreatedAt).IsRequired();
                comment.Property(c => c.IsVisible).IsRequired();
                comment.ToTable("ReportComments"); // Separate table for comments
            });
            builder.OwnsMany(r => r.StatusHistory, statusChange =>
            {
                statusChange.WithOwner().HasForeignKey("ReportId");
                statusChange.HasKey(sc => sc.Id); // Explicit key configuration
                statusChange.Property(sc => sc.Status).HasConversion<int>().IsRequired();
                statusChange.Property(sc => sc.ChangedAt).IsRequired();
                statusChange.Property(sc => sc.Reason).HasMaxLength(1000).IsRequired();
                statusChange.ToTable("ReportStatusChanges"); // Separate table for status changes
            });
        }
    }
}