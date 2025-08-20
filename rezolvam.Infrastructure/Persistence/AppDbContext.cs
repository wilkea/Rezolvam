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
using Microsoft.AspNetCore.Identity;

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

            // --- Seed pentru Admin ---
            var adminRoleId = Guid.NewGuid().ToString();
            var userRoleId = Guid.NewGuid().ToString();
            var adminUserId = Guid.NewGuid().ToString();

            // Rol Admin
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = adminRoleId,
                Name = "Admin",
                NormalizedName = "ADMIN"
            });
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = userRoleId,
                Name = "User",
                NormalizedName = "USER"
            });

            // User Admin
            var hasher = new PasswordHasher<ApplicationUser>();
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@example.com",
                NormalizedEmail = "ADMIN@EXAMPLE.COM",
                EmailConfirmed = true,
                FullName = "System Administrator",
                SecurityStamp = Guid.NewGuid().ToString("D"),
                PasswordHash = hasher.HashPassword(null, "Admin123") // parola hardcodata aici
            };

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);


            // Legăm utilizatorul de rol
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = adminRoleId,
                UserId = adminUserId
            });

            base.OnModelCreating(modelBuilder);
        }
    }


}
