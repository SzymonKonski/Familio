using System.Text;
using Application.Common.Interfaces;
using Domain.Entities;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Infrastructure.Services.AppSettings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailSettings>(configuration.GetSection("Email"));
        services.Configure<ClientAppSettings>(configuration.GetSection("ClientApp"));
        services.Configure<JwtSecurityTokenSettings>(configuration.GetSection("JwtSecurityToken"));

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services
            .AddIdentity<DomainUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders()
            .AddTokenProvider<CustomTokenProvider<DomainUser>>(nameof(CustomTokenProvider<DomainUser>));

        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = false;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            options.Lockout.MaxFailedAccessAttempts = 10;
            options.Lockout.AllowedForNewUsers = false;

            // User settings
            options.User.RequireUniqueEmail = true;
        });

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IEmailService, EmailService>();
        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IAzureStorage, AzureStorage>();
        services.AddTransient<IFileValidator, FileValidator>();
        services.AddTransient<IInvitationDataProtector, InvitationDataProtector>();
        services.AddTransient<IEmailCallbackService, EmailCallbackService>();
        services.AddTransient<IUserManagerService, UserManagerService>();
        services.AddTransient<IMobileCodeService, MobileCodeService>();
        services.AddTransient<IPathProvider, PathProvider>();

        var refreshTokenValidationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ClockSkew = TimeSpan.Zero,

            ValidIssuer = configuration["JwtSecurityToken:Issuer"],
            ValidAudience = configuration["JwtSecurityToken:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecurityToken:Key"]))
        };

        services.AddSingleton(refreshTokenValidationParams);

        services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                //o.RequireHttpsMetadata = false;
                //o.SaveToken = false;
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        // If the request is for our hub...
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/chatHub"))
                            // Read the token out of the query string
                            context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,

                    ValidIssuer = configuration["JwtSecurityToken:Issuer"],
                    ValidAudience = configuration["JwtSecurityToken:Audience"],
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSecurityToken:Key"]))
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("UpdateGroupDetail", policy => policy.RequireRole("Parent"));
        });

        return services;
    }
}