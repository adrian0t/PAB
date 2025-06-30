using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(string projectKey);
    Task<IEnumerable<ProjectListViewDTO>> GetAllAsync();
    Task<IEnumerable<ProjectListViewDTO>> GetAllByUserIdAsync(string username);
    Task<ProjectDetailstViewDTO?> GetByIdWithDetailsAsync(string projectKey);
    Task<ProjectDetailstViewDTO?> EnrichAsync(Project project);
    Task<List<ProjectDetailstViewDTO>> EnrichListAsync(IEnumerable<Project> projects);

    Task AddAsync(Project project);
    Task<int> CountAsync();
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);

    Task<bool> UserHasAccessAsync(string username, string projectKey);
    Task<bool> ExistsAsync(string projectKey);
    Task AddParticipantAsync(Project project, ApplicationUser user, ProjectRole role);
}
