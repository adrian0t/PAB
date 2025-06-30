using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;

namespace TaskFlow.API.Controllers;

[Authorize("isUser")]
[ApiController]
[Route("taskflow/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserContext _userContext;

    public TasksController(
        ITaskItemRepository taskItemRepository,
        IUserContext userContext)
    {
        _taskItemRepository = taskItemRepository;
        _userContext = userContext;
    }

    [HttpGet("mine")]
    public async Task<IActionResult> ListMyTasks()
    {
        var auth = await _userContext.GetAuthorizations(User, null);
        if (auth.UserId is null || !auth.IsUser)
            return Unauthorized();

        var tasksEnriched = await _taskItemRepository.GetByUsernameAsync(auth.UserId);
        return Ok(tasksEnriched);
    }

    [HttpGet("{key}/details")]
    public async Task<IActionResult> Details([FromRoute] string key)
    {
        var taskItem = await _taskItemRepository.GetByKeyAsync(key);
        var projectKey = taskItem?.Project?.Key;

        var auth = await _userContext.GetAuthorizations(User, projectKey);
        if (!auth.IsOwner && !auth.IsParticipant && !auth.IsAdmin)
            return Unauthorized();

        var taskItemEnriched = await _taskItemRepository.EnrichAsync(taskItem);
        return Ok(taskItemEnriched);
    }

    [HttpPatch("{key}")]
    public async Task<IActionResult> Edit([FromRoute] string key, [FromBody] TaskChangeDTO body)
    {
        var projectKey = key.Split("-")[0];
        var auth = await _userContext.GetAuthorizations(User, projectKey);
        if (!auth.IsOwner && !auth.IsParticipant && !auth.IsAdmin)
            return Unauthorized();

        var operations = new[] { "assign", "complete" };
        if (string.IsNullOrWhiteSpace(body.operation) ||
            !operations.Contains(body.operation) ||
            (body.operation == "assign" && string.IsNullOrWhiteSpace(body.value)))
            return BadRequest("Invalid operation or missing value.");

        var task = await _taskItemRepository.GetByKeyAsync(key);
        var updatedTask = await _taskItemRepository.UpdateAsync(task, body);

        return Ok(new
        {
            Message = "Changes applied",
            Task = updatedTask
        });
    }

    [HttpPatch("{key}/comment")]
    public async Task<IActionResult> Comment([FromRoute] string key, [FromBody] string body)
    {

        var task = await _taskItemRepository.GetByKeyAsync(key);
        if (task is null)
        {
            return NotFound($"Task with key '{key}' not found.");
        }

        var projectKey = task.Project?.Key;
        if (string.IsNullOrEmpty(projectKey))
        {
            return StatusCode(500, "Task is not associated with any project.");
        }

        var auth = await _userContext.GetAuthorizations(User, projectKey);


        if (!auth.IsOwner && !auth.IsParticipant && !auth.IsAdmin)
            return Unauthorized();

        var result = await _taskItemRepository.CommentAsync(body, key, auth.UserId);
        if (result is null)
            return BadRequest("Comment could not be added.");

        return Ok(result);
    }
}
