using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rezolvam.Domain.ReportComments;
using rezolvam.Domain.Reports;

namespace rezolvam.Infrastructure.Persistence.Configurations.Reports
{
    public class ReportCommentConfiguration : IEntityTypeConfiguration<ReportComment>
    {
        public void Configure(EntityTypeBuilder<ReportComment> builder)
        {
            builder.ToTable("ReportComments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.AuthorId).IsRequired();
            builder.Property(c => c.Message).IsRequired().HasMaxLength(2000);
            builder.Property(c => c.Type).HasConversion<string>().IsRequired();
            builder.Property(c => c.CreatedAt).IsRequired();
            builder.Property(c => c.IsVisible).IsRequired();
            builder.Property(c => c.ReportId).IsRequired();

            builder.HasOne<Report>()
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.ReportId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
