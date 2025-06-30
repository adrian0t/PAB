using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TaskFlow.Application.Repositories;
using TaskFlow.Domain.Entities;
using TaskFlow.Application.Services;
using TaskFlow.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Data;
using Microsoft.AspNetCore.Identity.UI;

namespace TaskFlow.Admin.Pages
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IUserContext _userContext;

        public DashboardModel(
            IProjectRepository projectRepository,
            ITaskItemRepository taskItemRepository,
            IUserRepository userRepository,
            IUserContext userContext
        )
        {
            _userRepository = userRepository;
            _taskItemRepository = taskItemRepository;
            _projectRepository = projectRepository;
            _userContext = userContext;
        }

        public string Username { get; set; }
        public IEnumerable<ProjectListViewDTO> Projects { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public bool IsAdmin { get; set; }

        public async Task OnGetAsync()
        {
            await PopulateField();
        }

        public async Task<IActionResult> OnPostCreateProjectAsync(
            [FromForm] ProjectListViewDTO NewProject
            //string ProjectKey
        )
        {
            await PopulateField();
            bool exists = await _projectRepository.ExistsAsync(NewProject.ProjectKey);
            //bool exists = await _projectRepository.ExistsAsync(ProjectKey);

            if (exists)
            {
                TempData["ErrorMessage"] = "Project key is already taken.";
                return LocalRedirect("/");
            }

            Project projectFinalized = new Project("jakiś-klucz", "jakaś-nazwa");

            await _projectRepository.AddAsync(projectFinalized);

            TempData["SuccessMessage"] = "Project created.";
            return LocalRedirect("/");
        }
        public async Task<IActionResult> OnPostAssignUserToProjectAsync(
            [FromForm] ApplicationUser user,
            [FromForm] ProjectListViewDTO project,
            [FromForm] ProjectRole role)
        {
        
            if (user is null || project is null)
            {
                TempData["ErrorMessage"] = "Invalid user, project, or role.";
                return RedirectToAction("/Dashboard");
            }
            Project projectFinalized = await _projectRepository.GetByIdAsync(project.ProjectKey);

            await _projectRepository.AddParticipantAsync(projectFinalized, user, role);

            TempData["SuccessMessage"] = $"User {user.UserName} assigned to project {project.ProjectKey} as {(role == ProjectRole.Owner ? "Owner" : "Participant")} .";
            return RedirectToAction("/Dashboard");
        }

        private async Task PopulateField()
        {
            var auth = await _userContext.GetAuthorizations(User, null);

            this.Username = auth.UserId;
            this.IsAdmin = auth.IsAdmin;

            if (this.IsAdmin)
                this.Projects = await _projectRepository.GetAllAsync();
            else
            {
                this.Projects = await _projectRepository.GetAllByUserIdAsync(this.Username);
            }

            if (this.IsAdmin)
                this.Users = await _userRepository.GetAllAsync();
        }
    }
}
