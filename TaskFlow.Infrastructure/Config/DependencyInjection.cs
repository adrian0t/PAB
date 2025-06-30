using Microsoft.Extensions.DependencyInjection;
using TaskFlow.Application.Repositories;
using TaskFlow.Application.Services;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using TaskFlow.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Builder;
using TaskFlow.Infrastructure.Authorization;
using TaskFlow.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace TaskFlow.Infrastructure.Config;

public static class DependencyInjection
{
    private static readonly InMemoryDatabaseRoot _dbRoot = new();
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
           options.UseInMemoryDatabase("TaskFlowDb", _dbRoot));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Lockout.MaxFailedAccessAttempts = 999;
            options.Lockout.AllowedForNewUsers = false;
            options.SignIn.RequireConfirmedAccount = false;
            options.SignIn.RequireConfirmedEmail = false;
            options.SignIn.RequireConfirmedPhoneNumber = false;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();
       
        
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITokenCreator, TokenCreator>();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ILoginHandler, LoginHandler>();

        return services;
    }

    public async static Task SeedDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<AppDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await DbSeeder.SeedAsync(context, userManager, roleManager);        
    }
    public static IServiceCollection AddPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("isOwner", policy =>
                policy.Requirements.Add(new ProjectRoleRequirement(ProjectRole.Owner)));
            options.AddPolicy("isParticipant", policy =>
                policy.Requirements.Add(new ProjectRoleRequirement(ProjectRole.Participant)));
            options.AddPolicy("isAdmin", policy =>
                policy.Requirements.Add(new GlobalRoleRequirement("Admin")));
            options.AddPolicy("isUser", policy =>
               policy.Requirements.Add(new GlobalRoleRequirement("User")));
        });
        services.AddScoped<IAuthorizationHandler, ProjectRoleHandler>();
        services.AddScoped<IAuthorizationHandler, GlobalRoleHandler>();

        return services;
    }


}
