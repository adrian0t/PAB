using System.ComponentModel.DataAnnotations;
using TaskFlow.Domain.Extensions;

namespace TaskFlow.Domain.Entities;

public enum TaskStatus
{
    [Display(Name = "New")]
    New,
    [Display(Name = "In Progress")]
    InProgress,
    [Display(Name = "Done")]
    Done
}

public class TaskItem
{
    public string TaskKey { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; } = false;
    public TaskStatus Status { get; set; } = TaskStatus.New;
    public string StatusText => Status.GetDisplayName();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DoneAt { get; set; } = null;
    // Foreign key to Project
    public Project Project { get; set; } = null!;
    public ApplicationUser? Assignee {  get; set; }
    //public string? Assignee { get; set; }
    public ICollection<Comment>? Comments { get; set; }
}