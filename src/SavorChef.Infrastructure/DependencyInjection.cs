using System.Text;
using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.FileStorage;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SavorChef.Application.Common.Interfaces;
using SavorChef.Application.Common.Models;
using SavorChef.Infrastructure.Data;
using SavorChef.Infrastructure.Data.Interceptors;
using SavorChef.Infrastructure.Email;
using SavorChef.Infrastructure.Identity;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        AddDatabaseServices(builder);
        AddAuthenticationAndAuthorization(builder);
        AddIdentityServices(builder);
        AddFileStorageServices(builder);
        AddMiscellaneousServices(builder);
    }

    private static void AddDatabaseServices(IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("SavorChefDb");
        Guard.Against.Null(connectionString, message: "Connection string 'SavorChefDb' not found.");

        builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        builder.Services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        builder.Services.AddDbContext<ApplicationDataContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());
            options.UseNpgsql(connectionString);
        });

        builder.Services.AddScoped<IApplicationDataContext>(provider => provider.GetRequiredService<ApplicationDataContext>());
        builder.Services.AddScoped<ApplicationDataContextInitializer>();

        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Valkey");
            options.InstanceName = "SavorChef:";
        });
    }

    private static void AddAuthenticationAndAuthorization(IHostApplicationBuilder builder)
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
        Guard.Against.Null(jwtSettings, message: "JWT settings not found.");

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
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        builder.Services.AddAuthorizationBuilder();
        builder.Services.AddSingleton(jwtSettings);
    }

    private static void AddIdentityServices(IHostApplicationBuilder builder)
    {
        builder.Services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDataContext>();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.User.RequireUniqueEmail = true;
        });

        builder.Services.AddTransient<IIdentityService, IdentityService>();
    }

    private static void AddFileStorageServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IAmazonS3>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<FileStorageSettings>>().Value;

            var config = new AmazonS3Config
            {
                ServiceURL = settings.ServiceUrl,
                ForcePathStyle = true,
                RegionEndpoint = RegionEndpoint.USEast1 // TODO: Make this configurable
            };

            return new AmazonS3Client(settings.AccessKey, settings.SecretKey, config);
        });
    }

    private static void AddMiscellaneousServices(IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IEmailSender<ApplicationUser>, NoOpEmailSender>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddSingleton<ITokenRepository, TokenRepository>();
        builder.Services.AddSingleton(TimeProvider.System);
    }
}