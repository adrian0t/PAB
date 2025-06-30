using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Authorization;
public class GlobalRoleRequirement : IAuthorizationRequirement
{
    public string RequiredRole { get; }

    public GlobalRoleRequirement(string projectRole)
    {
        RequiredRole = projectRole;
    }
}