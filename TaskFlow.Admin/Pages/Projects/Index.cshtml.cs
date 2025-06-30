using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Infrastructure.Config;

namespace TaskFlow.Admin.Pages.Projects;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IProjectRepository _projectRepository;

    public IndexModel(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public IList<ProjectDetailstViewDTO> Projects { get;set; } = default!;

    public async Task OnGetAsync()
    {
        var projects = await _projectRepository.GetAllAsync(); 

        var keys = projects.Select(p => p.ProjectKey).ToList();

        var projectEntities = await Task.WhenAll(
            keys.Select(key => _projectRepository.GetByIdAsync(key))
        );

        var projectsFinalized = await _projectRepository.EnrichListAsync(projectEntities.Where(p => p != null)!);

        Projects = projectsFinalized;
    }
}
