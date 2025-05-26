using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AutoMapper;
using rezolvam.Application.MappingProfiles;

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
            services.AddAutoMapper(typeof(ReportMappingProfile));
            
            return services;
        }
        
    }
    
}
