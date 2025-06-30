using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.DTOs;

public class ProjectListViewDTO
{
    public string ProjectKey { get; set; } = null!;
    public List<string> Owners { get; set; } = null!;
}