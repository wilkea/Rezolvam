using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Infrastructure.Persistence;
using rezolvam.Infrastructure.Persistence.Configurations.Reports;
using Microsoft.Extensions.Configuration;
using rezolvam.Application.Interfaces;

namespace rezolvam.Infrastructure
{
    public static class InfrastructureServiceCollectionExtentions
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            return services;
        }
    }
}
