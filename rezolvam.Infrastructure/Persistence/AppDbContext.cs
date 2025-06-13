using Microsoft.EntityFrameworkCore;
using rezolvam.Infrastructure.Persistence.Configurations.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using rezolvam.Application.Interfaces;
using rezolvam.Domain.Reports;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using rezolvam.Domain.Common;
using rezolvam.Domain.ReportComments;
using rezolvam.Domain.ReportPhotos;
using rezolvam.Domain.Report.StatusChanges;

namespace rezolvam.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Report> Reports { get; set; }
        public DbSet<ReportPhoto> ReportPhotos { get; set; }
        public DbSet<ReportComment> ReportComments { get; set; }
        public DbSet<StatusChange> StatusChanges { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ReportConfiguration());
            modelBuilder.ApplyConfiguration(new ReportCommentConfiguration());
            modelBuilder.ApplyConfiguration(new ReportPhotoConfiguration());
            modelBuilder.ApplyConfiguration(new StatusChangeConfiguration());

            base.OnModelCreating(modelBuilder);
        } 
    }

    
}
