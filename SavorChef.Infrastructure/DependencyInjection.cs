using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SavorChef.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Infrastructure.Data;
using SavorChef.Infrastructure.Data.Interceptors;
using SavorChef.Infrastructure.Email;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SavorChefDb");
        Guard.Against.Null(connectionString, message: "Connection string 'SavorChefDb' not found.");

        var jwtSettings = builder.Configuration
            .GetSection("Jwt")
            .Get<JwtSettings>();
        Guard.Against.Null(jwtSettings, message: "JWT settings not found.");
        
        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDataContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });
        
        builder.Services.AddScoped<IApplicationDataContext>(provider => provider.GetRequiredService<ApplicationDataContext>());

        builder.Services.AddScoped<ApplicationDataContextInitializer>();
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        builder.Services.AddAuthorizationBuilder();
        builder.Services.AddSingleton(jwtSettings);


        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDataContext>();
        
        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
        });
        
        // no email sender
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, NoOpEmailSender>();
        
        builder.Services.AddSingleton(TimeProvider.System);
        builder.Services.AddTransient<IIdentityService, IdentityService>();
    }
}