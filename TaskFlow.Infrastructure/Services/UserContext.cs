using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities; 
using TaskFlow.Application.Repositories;

namespace TaskFlow.Infrastructure.Services;

public class UserContext : IUserContext
{
    private readonly IServiceProvider _provider;
    public UserContext(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<UserContextDTO> GetAuthorizations(ClaimsPrincipal user, string? projectKey)
    {
        using var scope = _provider.CreateScope();
        var authService = scope.ServiceProvider.GetRequiredService<IAuthorizationService>();
        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();

        UserContextDTO result = new UserContextDTO();

        var ID = user.FindFirstValue(ClaimTypes.Name);
        var isAdminResult = await authService.AuthorizeAsync(user, "isAdmin");
        var isUserResult = await authService.AuthorizeAsync(user, "isUser");

        result = new UserContextDTO
        {
            UserId = ID,
            IsAdmin = isAdminResult.Succeeded,
            IsUser = isUserResult.Succeeded,
        };

        if (string.IsNullOrEmpty(projectKey))
        {
            return result;
        }

        var projectResource = await projectRepository.GetByIdAsync(projectKey);

        if (projectResource is null)
        {
            result.IsOwner = false;
            result.IsParticipant = false;
            return result;
        }

        var isOwnerResult = await authService.AuthorizeAsync(user, projectResource, "isOwner");
        var isParticipantResult = await authService.AuthorizeAsync(user, projectResource, "isParticipant");

        result.IsOwner = isOwnerResult.Succeeded;
        result.IsParticipant = isParticipantResult.Succeeded;

        return result;
    }
}