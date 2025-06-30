using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.DTOs;

public class TaskItemDetailsDTO
{
    [Display(Name = "Task Key")]
    public string TaskKey { get; set; } = null!;
    [Display(Name = "Title")]
    public string Title { get; set; } = null!;
    [Display(Name = "Description")]
    public string? Description { get; set; }
    [Display(Name = "Status")]
    public string StatusText { get; set; } = null!;
    [Display(Name = "Completed")]
    public bool IsCompleted { get; set; } = false;
    [Display(Name = "Assigned to")]
    public string? Assignee { get; set; } = null;
    [Display(Name = "Created at")]
    public DateTime CreatedAt { get; set; } = new();
    [Display(Name = "Done at")]
    public DateTime? DoneAt { get; set; } = null;
    public List<CommentDTO> Comments { get; set; } = null!;

}
