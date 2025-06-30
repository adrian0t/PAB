using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;

namespace TaskFlow.API.Controllers;

[Authorize(Policy = "IsUser")]
[ApiController]
[Route("taskflow/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IUserContext _userContext;

    public ProjectsController(
        IProjectRepository projectRepository,
        ITaskItemRepository taskItemRepository,
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _userRepository = userRepository;
        _taskItemRepository = taskItemRepository;
        _projectRepository = projectRepository;
        _userContext = userContext;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var auth = await _userContext.GetAuthorizations(User, null);

        var result = auth.IsAdmin
            ? await _projectRepository.GetAllAsync()
            : null;

        return Ok(result);
    }

    [HttpGet("{key}")]
    public async Task<IActionResult> Details([FromRoute] string key)
    {
        var auth = await _userContext.GetAuthorizations(User, null);

        if (!auth.IsOwner && !auth.IsParticipant && !auth.IsAdmin)
            return Unauthorized();

        var result = await _projectRepository.GetByIdWithDetailsAsync(key.ToUpper());
        return Ok(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] NewProjectDTO body)
    {
        var auth = await _userContext.GetAuthorizations(User, null);

        string key = body.projectKey.ToUpper();
        List<string> usernames = body.userNames;

        if (await _projectRepository.ExistsAsync(key))
            return BadRequest("Project key already in use!");

        var users = await _userRepository.GetListByUsernameAsync(usernames);
        var foundUsernames = users.Select(u => u.UserName).ToList();
        var missingUsernames = usernames.Except(foundUsernames).ToList();


        var project = new Project(key, key);

        var userProjects = users.Select(u => new UserProject
        {
            User = u,
            Project = project, 
            Role = ProjectRole.Participant
        }).ToList();

        project.UserProjects = userProjects;


        await _projectRepository.AddAsync(project);

        return Ok(new
        {
            result = "Project created",
            NewKey = key,
            UsersAdded = foundUsernames,
            UsersNotFound = missingUsernames
        });
    }

    [HttpDelete("{key}/delete")]
    public async Task<IActionResult> Delete([FromRoute] string key)
    {
        var auth = await _userContext.GetAuthorizations(User, null);

        if (!auth.IsOwner && !auth.IsAdmin)
            return Unauthorized();

        key = key.ToUpper();
        var project = await _projectRepository.GetByIdAsync(key);

        if (project == null)
            return BadRequest("Project does not exist!");

        await _projectRepository.DeleteAsync(project);

        return Ok(new
        {
            result = "Project deleted",
            OldKey = key
        });
    }

    [HttpPost("{key}/AddTask")]
    public async Task<IActionResult> AddTask(
        [FromRoute] string key,
        [FromBody] TaskItemCreateDTO body)
    {
        var auth = await _userContext.GetAuthorizations(User, key);

        if (!auth.IsOwner && !auth.IsParticipant && !auth.IsAdmin)
            return Unauthorized();

        string? created = await _taskItemRepository.AddAsync(body, key);

        if (created is null)
            return BadRequest("Parameters could not be resolved");

        return Ok(new
        {
            result = "TaskItem created",
            Key = created
        });
    }
}
