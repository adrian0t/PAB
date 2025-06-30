using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Infrastructure.Config;

namespace TaskFlow.Infrastructure.Authorization;

public class ProjectRoleHandler : AuthorizationHandler<ProjectRoleRequirement, string>
{
    private readonly AppDbContext _context;

    public ProjectRoleHandler(AppDbContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ProjectRoleRequirement requirement,
        string projectKey)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return;

        var isAdmin = context.User.IsInRole("Admin");

        var hasRole = await _context.UserProjects.AnyAsync(up =>
            up.Project.Key == projectKey &&
            up.User.UserName == userId &&
            up.Role == requirement.RequiredRole);

        if (isAdmin || hasRole)
            context.Succeed(requirement);
    }
}