using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Repositories;
using TaskFlow.Infrastructure.Config;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Admin.Pages.Projects;

[Authorize]
public class DeleteModel : PageModel
{
    private readonly IProjectRepository _projectRepository;

    public DeleteModel(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    [BindProperty]
    public Project Project { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _projectRepository.GetByIdAsync(id);

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

    public async Task<IActionResult> OnPostAsync(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _projectRepository.GetByIdAsync(id);
        if (project != null)
        {
            Project = project;
            await _projectRepository.DeleteAsync(Project);
        }

        return RedirectToPage("./Index");
    }
}
