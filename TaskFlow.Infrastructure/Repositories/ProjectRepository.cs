using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Config;

namespace TaskFlow.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;

    public ProjectRepository(AppDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task AddAsync(Project project)
    {
        await _context.AddAsync(project);
        await _context.SaveChangesAsync();
    }

    public async Task<int> CountAsync() => await _context.Projects.CountAsync();

    public async Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<ProjectListViewDTO>> GetAllAsync()
    {
        return await _context.Projects.Select(ToListView()).ToListAsync();

    }

    public async Task<IEnumerable<ProjectListViewDTO>> GetAllByUserIdAsync(string username)
    {
        return await _context.Projects
            .Where(p => p.UserProjects.Any(up => up.User.UserName == username))
            .Select(ToListView())
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(string projectKey)
    {
        return await _context.Projects.FirstOrDefaultAsync(p => p.Key == projectKey.ToUpper());
    }

    public async Task<ProjectDetailstViewDTO?> GetByIdWithDetailsAsync(string projectKey)
    {
        return await _context.Projects
            .Where(p => p.Key == projectKey.ToUpper())
            .Select(ToDetailsView())
            .FirstOrDefaultAsync();
    }

    public Task UpdateAsync(Project project)
    {
        throw new NotImplementedException();
    }

    public async Task AddParticipantAsync(Project project, ApplicationUser user, ProjectRole role)
    {
        project.UserProjects.Add(new UserProject
        {
            Project = project,
            User = user,
            Role = role
        });
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UserHasAccessAsync(string username, string projectKey)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        return await _context.UserProjects.AnyAsync(up => up.Project.Key == projectKey.ToUpper() && up.User == user);
    }

    public async Task<bool> ExistsAsync(string projectKey)
    {
        return await _context.Projects.AnyAsync(p => p.Key == projectKey.ToUpper());
    }

    public async Task<ProjectDetailstViewDTO?> EnrichAsync(Project project)
    {
        return await GetByIdWithDetailsAsync(project.Key);
    }

    public async Task<List<ProjectDetailstViewDTO>> EnrichListAsync(IEnumerable<Project> projects)
    {
        var keys = projects.Select(p => p.Key);
        return await _context.Projects
            .Where(p => keys.Contains(p.Key))
            .Select(ToDetailsView())
            .ToListAsync();
    }

    private static Expression<Func<Project, ProjectDetailstViewDTO>> ToDetailsView()
    {
        return p => new ProjectDetailstViewDTO
        {
            ProjectKey = p.Key,
            Participants = p.UserProjects.Select(up => new ParticipantDTO
            {
                User = up.User.UserName,
                Role = up.Role.ToString()
            }).ToList(),
            Tasks = p.Tasks.Select(t => new TaskItemDetailsDTO
            {
                TaskKey = t.TaskKey,
                Title = t.Title,
                Description = t.Description,
                StatusText = t.StatusText,
                IsCompleted = t.IsCompleted,
                Assignee = t.Assignee != null ? t.Assignee.UserName : string.Empty,
                CreatedAt = t.CreatedAt,
                DoneAt = t.DoneAt
            }).ToList()
        };
    }
    private static Expression<Func<Project, ProjectListViewDTO>> ToListView()
    {
        return p => new ProjectListViewDTO
        {
            ProjectKey = p.Key,
            Owners = p.UserProjects
                .Where(up => up.Role == ProjectRole.Owner)
                .Select(up => up.User.UserName)
                .ToList()
        };
    }
}
