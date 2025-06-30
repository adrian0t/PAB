using Microsoft.AspNetCore.Authorization;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Authorization;

public class ProjectRoleRequirement : IAuthorizationRequirement
{
    public ProjectRole RequiredRole { get; }

    public ProjectRoleRequirement(ProjectRole projectRole)
    {
        RequiredRole = projectRole;
    }
}