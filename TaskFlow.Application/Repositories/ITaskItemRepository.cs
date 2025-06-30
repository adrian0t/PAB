using TaskFlow.Domain.Entities;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Repositories;

public interface ITaskItemRepository
{
    Task<string?> AddAsync(TaskItemCreateDTO taskDTO, string projectKey);
    Task<int> CountAsync();
    Task DeleteAsync(TaskItem task);
    Task<List<TaskItemDetailsDTO>> GetByUsernameAsync(string username);
    Task<List<TaskItemDetailsDTO>> GetAllByProjectKeyAsync(string projectKey);
    Task<TaskItem?> GetByKeyAsync(string taskKey);
    Task<TaskItem?> UpdateAsync(TaskItem task, TaskChangeDTO body);
    Task<CommentDTO?> CommentAsync(string text, string taskKey, string username);
    Task<TaskItemDetailsDTO> EnrichAsync(TaskItem task);
    Task<List<TaskItemDetailsDTO>> EnrichListAsync(List<TaskItem> tasks);
}