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

namespace rezolvam.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Report> Reports { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ReportConfiguration());

            base.OnModelCreating(modelBuilder);
        } 
    }
}
