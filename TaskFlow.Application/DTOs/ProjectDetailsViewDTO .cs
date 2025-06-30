using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.DTOs;

public class ProjectDetailstViewDTO
{
    [Display(Name = "Project Key")]
    public string ProjectKey { get; set; } = null!;
    [Display(Name = "Participants")]
    public List<ParticipantDTO> Participants { get; set; } = new();
    [Display(Name = "Tasks")]
    public List<TaskItemDetailsDTO> Tasks { get; set; } = new();
}