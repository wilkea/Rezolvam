using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AutoMapper;
using rezolvam.Application.Services.Helpers;

namespace rezolvam.Application

{
    public static class ApplicationServiceCollectionExtentions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(ApplicationServiceCollectionExtentions).Assembly);
            });

            services.AddScoped<IReportServiceHelper, ReportServiceHelper>();
            return services;
        }
        
    }
    
}
