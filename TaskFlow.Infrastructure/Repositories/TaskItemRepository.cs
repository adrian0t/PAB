using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Config;

namespace TaskFlow.Infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly AppDbContext _context;
    private readonly IUserRepository _userRepository;
    private readonly IProjectRepository _projectRepository;

    public TaskItemRepository(AppDbContext context, IUserRepository userRepository, IProjectRepository projectRepository)
    {
        _context = context;
        _userRepository = userRepository;
        _projectRepository = projectRepository;
    }

    public async Task<string?> AddAsync(TaskItemCreateDTO taskDTO, string projectKey)
    {
        var project = await _projectRepository.GetByIdAsync(projectKey.ToUpper());
        if (project == null) return null;

        var assignee = await _userRepository.GetByUsernameAsync(taskDTO.Assignee);

        var task = new TaskItem
        {
            TaskKey = project.GenerateNextTaskKey(),
            Title = taskDTO.Title,
            Description = taskDTO.Description,
            Project = project,
            Assignee = assignee,
            Status = TaskFlow.Domain.Entities.TaskStatus.New,
            CreatedAt = DateTime.UtcNow
        };

        await _context.TaskItems.AddAsync(task);
        await _context.SaveChangesAsync();

        return task.TaskKey;
    }

    public async Task<int> CountAsync() => await _context.TaskItems.CountAsync();

    public async Task DeleteAsync(TaskItem task)
    {
        _context.TaskItems.Remove(task);
        await _context.SaveChangesAsync();
    }

    public async Task<List<TaskItemDetailsDTO>> GetByUsernameAsync(string username)
    {
        return await _context.TaskItems
            .Where(ti => ti.Assignee.UserName == username)
            .Select(ToDetailsDTO())
            .ToListAsync();
    }

    public async Task<List<TaskItemDetailsDTO>> GetAllByProjectKeyAsync(string projectKey)
    {
        return await _context.TaskItems
            .Where(t => t.Project.Key == projectKey.ToUpper())
            .Select(ToDetailsDTO())
            .ToListAsync();
    }

    public async Task<TaskItem?> GetByKeyAsync(string taskKey)
    {
        return await _context.TaskItems
            .Include(t => t.Project)
            .Include(t => t.Assignee)
            .FirstOrDefaultAsync(t => t.TaskKey == taskKey.ToUpper());
    }

    public async Task<TaskItem?> UpdateAsync(TaskItem task, TaskChangeDTO body)
    {
        switch (body.operation)
        {
            case "assign":
                task.Assignee = await _userRepository.GetByUsernameAsync(body.value!);
                break;
            case "complete":
                task.Status = TaskFlow.Domain.Entities.TaskStatus.Done;
                task.IsCompleted = true;
                task.DoneAt = DateTime.UtcNow;
                break;
        }

        await _context.SaveChangesAsync();
        return task;
    }

    public async Task<CommentDTO?> CommentAsync(string text, string taskKey, string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        var task = await GetByKeyAsync(taskKey.ToUpper());
        if (user is null || task is null || task.IsCompleted == true) return null;

        var comment = new Comment
        {
            Text = text,
            User = user,
            Task = task
        };

        await _context.Comments.AddAsync(comment);
        await NotifyAssignee(comment, task, user);
        await _context.SaveChangesAsync();

        return new CommentDTO
        {
            Text = comment.Text,
            UserName = user.UserName,
            CommentedAt = comment.CommentedAt,
            TaskKey = task.TaskKey
        };
    }

    private async Task NotifyAssignee(Comment comment, TaskItem task, ApplicationUser commenter)
    {
        if (commenter == task.Assignee) return;

        await _context.UnreadComments.AddAsync(new UserUnreadComment
        {
            User = task.Assignee!,
            Comment = comment
        });
    }

    public async Task<TaskItemDetailsDTO> EnrichAsync(TaskItem task)
    {
        return await _context.TaskItems
            .Where(t => t.TaskKey == task.TaskKey)
            .Include(t => t.Comments)
            .Select(ToDetailsDTO())
            .FirstOrDefaultAsync()!;
    }

    public async Task<List<TaskItemDetailsDTO>> EnrichListAsync(List<TaskItem> tasks)
    {
        var keys = tasks.Select(t => t.TaskKey).ToList();
        return await _context.TaskItems
            .Where(t => keys.Contains(t.TaskKey))
            .Include(t => t.Comments)
            .Select(ToDetailsDTO())
            .ToListAsync();
    }

    private static Expression<Func<TaskItem, TaskItemDetailsDTO>> ToDetailsDTO()
    {
        return t => new TaskItemDetailsDTO
        {
            TaskKey = t.TaskKey,
            Title = t.Title,
            Description = t.Description,
            StatusText = t.StatusText,
            IsCompleted = t.IsCompleted,
            Assignee = t.Assignee != null ? t.Assignee.UserName : string.Empty,
            CreatedAt = t.CreatedAt,
            DoneAt = t.DoneAt,
            Comments = t.Comments.Select(comment => new CommentDTO
            {
                Text = comment.Text,
                UserName = comment.User.UserName,
                CommentedAt = comment.CommentedAt,
                TaskKey = comment.Task.TaskKey
            }).ToList()
        };
    }
}
