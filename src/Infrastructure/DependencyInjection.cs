using WebApi.Application.Common.Interfaces;
using WebApi.Domain.Constants;
using WebApi.Infrastructure.Data;
using WebApi.Infrastructure.Data.Interceptors;
using WebApi.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces;
using Infrastructure.Identity;
using WebApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using WebApi.Infrastructure.OptionsSetup;
using WebApi.Infrastructure.Permissions;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider
            => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddTransient<IIdentityService, IdentityService>();
        //services.AddTransient<IIdentityRoleService, IdentityRoleService>();
        services.AddTransient<IAuthAccountService, AuthAccountService>();
        services.AddTransient<IJwtProvider, JwtProvider>();

        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        //services.AddAuthentication()
        //    .AddBearerToken(IdentityConstants.BearerScheme);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer();

        services.AddAuthorizationBuilder();

        services
            .AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddApiEndpoints();

        services.AddSingleton(TimeProvider.System);
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));
            //options.AddPolicy(Policies.CanView, policy => policy.AddRequirements(new PermissionRequirement(Policies.CanView)));

        });

        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        // For dynamically create policy if not exist
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();


        return services;
    }
}
