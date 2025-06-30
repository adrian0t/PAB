using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskFlow.Application.DTOs;
using TaskFlow.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TaskFlow.Application.Repositories;
using TaskFlow.Application.Services;

namespace TaskFlow.API.Controllers;

[ApiController]
[Route("taskflow/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITokenCreator _tokenCreator;


    public AuthController(
        IProjectRepository projectRepository,
        ITaskItemRepository taskItemRepository,
        IUserRepository userRepository,
        ITokenCreator tokenCreator
    )
    {
        _userRepository = userRepository;
        _taskItemRepository = taskItemRepository;
        _projectRepository = projectRepository;
        _tokenCreator = tokenCreator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDTO body
    )
    {
        ApplicationUser? user = await _userRepository.GetByUsernameAsync(body.userName);

        string? token = await _tokenCreator.GenerateJwtTokenAsync(user, body.password);

        if (token is null) return Unauthorized();

        return Ok(new
        {
            bearer = token
        });
    }
}
