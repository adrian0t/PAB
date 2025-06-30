using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Infrastructure.Config;

namespace TaskFlow.Admin.Pages.Projects;

[Authorize]
public class DetailsModel : PageModel
{
    private readonly IProjectRepository _projectRepository;

    public DetailsModel(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public ProjectDetailstViewDTO Project { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _projectRepository.GetByIdWithDetailsAsync(id);
        
        if (project == null)
        {
            return NotFound();
        }

        else
        {
            Project = project;
        }
        return Page();
    }
}
