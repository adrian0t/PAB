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
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Config;

namespace TaskFlow.Admin.Pages.Users;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IUserRepository _userRepository;

    public IndexModel(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public List<ApplicationUser> Users { get;set; } = new List<ApplicationUser>();

    [BindProperty]
    public LoginDTO LoginDTO { get; set; } = new LoginDTO();
    public async Task OnGetAsync()
    {
        Users = await _userRepository.GetAllAsync();
    }
    public async Task<IActionResult> OnPostAddAsync()
    {
        Users = await _userRepository.GetAllAsync();

        bool exists = await _userRepository.ExistsAsync(LoginDTO.userName);

        if (exists)
        {
            TempData["ErrorMessage"] = "Username is already taken.";
            return LocalRedirect("/Users/Index");
        }

        await _userRepository.CreateAsync(LoginDTO);

        TempData["SuccessMessage"] = "User has been created.";
        return LocalRedirect("/Users/Index");
    }
}
