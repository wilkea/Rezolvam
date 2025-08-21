using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rezolvam.Domain.Reports.Interfaces;
using rezolvam.Infrastructure.Persistence;
using rezolvam.Infrastructure.Persistence.Configurations.Reports;
using Microsoft.Extensions.Configuration;
using rezolvam.Application.Interfaces;
using rezolvam.Domain.Common;
using Microsoft.AspNetCore.Identity;
using rezolvam.Application.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Rezolvam.Application.Interfaces;
using rezolvam.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;


namespace rezolvam.Infrastructure
{
    public static class InfrastructureServiceCollectionExtentions
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // services.AddScoped<IFileStorageService, LocalFileStorageService>();
            services.AddScoped<IFileStorageService, AzureBlobStorageService>();
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            // Configure JWT settings first
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null)
            {
                throw new ArgumentNullException("JwtSettings section is not configured properly.");
            }

            // Add Identity with roles
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            // Configure multiple authentication schemes properly
            services.AddAuthentication(options =>
            {
                // Set Identity's cookie as default for MVC
                options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
                options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.Request.Cookies["jwt"];
                        if (!string.IsNullOrEmpty(token))
                            context.Token = token;
                        return Task.CompletedTask;
                    },
                };
            });

            // Configure Identity's cookie options
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
            });

            // Set up authorization policies for both schemes
            services.AddAuthorization(options =>
            {
                // Default policy uses Identity cookies
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(IdentityConstants.ApplicationScheme)
                    .Build();

                // JWT policy for API endpoints
                options.AddPolicy("JwtPolicy", new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .Build());
            });

            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
