using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.DTOs;

public class TaskItemCreateDTO
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public string? Assignee { get; set; } = null;
}
