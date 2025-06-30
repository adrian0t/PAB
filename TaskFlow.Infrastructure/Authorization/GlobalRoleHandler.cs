using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TaskFlow.Infrastructure.Config;

namespace TaskFlow.Infrastructure.Authorization;
public class GlobalRoleHandler : AuthorizationHandler<GlobalRoleRequirement>
{
    private readonly AppDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;

    public GlobalRoleHandler(AppDbContext context, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _roleManager = roleManager;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        GlobalRoleRequirement requirement
    )
    {
        var user = context.User;

        if  (
                ( user.Identity is { IsAuthenticated: true } && user.IsInRole(requirement.RequiredRole) )
                    || 
                ( user.Identity is { IsAuthenticated: true } && user.IsInRole("Admin")) )
            {
            context.Succeed(requirement);
        }
    }   
}