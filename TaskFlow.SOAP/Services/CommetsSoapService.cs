using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Config;
using TaskFlow.Infrastructure.Repositories;
using TaskFlow.Infrastructure.Services;
using static System.Net.Mime.MediaTypeNames;


namespace TaskFlow.SOAP.Services;

public class CommentsSoapService : ICommentsSoapService
{
    private readonly IUserRepository _userRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenCreator _tokenCreator;
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly AppDbContext _context;
    public CommentsSoapService(
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor,
        ITokenCreator tokenCreator,
        ITaskItemRepository taskItemRepository,
        AppDbContext context
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _tokenCreator = tokenCreator;
        _taskItemRepository = taskItemRepository;
        _context = context;
    }

   // public List<string> DumpComments() => new() { "test" };

    public async Task<List<CommentDTO>> DumpCommentsAsync()
    {
        ApplicationUser user = await AuthenticateAsync();

        List<UserUnreadComment> unreadComments = user.UnreadComments.ToList();

        List<Comment> comments = unreadComments.Select(uc => uc.Comment).ToList();

        _context.Set<UserUnreadComment>().RemoveRange(unreadComments);

        List<CommentDTO> result = comments.Select(c => new CommentDTO
        {
            Text = c.Text,
            UserName = c.User.UserName,
            CommentedAt = c.CommentedAt,
            TaskKey = c.Task.TaskKey
        }).ToList();    


        return result;
    }
    private async Task<ApplicationUser> AuthenticateAsync()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            throw new UnauthorizedAccessException("Missing or malformed Authorization header.");
        var token = authHeader["Bearer ".Length..].Trim();

        var claimsPrincipal = await _tokenCreator.DecodeToken(token);
        string? userName = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value;
        ApplicationUser? user = await _userRepository.GetByUsernameAsync(userName);
        
        if (user is null)
            throw new UnauthorizedAccessException("Token is invalid.");
        //await SeedAsync(user);
        return user;
    }

    //private async Task SeedAsync(ApplicationUser user)
    //{
    //    var mytasks = await _taskItemRepository.GetByUsernameAsync(user.UserName);
    //    var userlist = await _userRepository.GetAllAsync();
    //    // Create some comments for that task
    //    var comment1 = new Comment
    //    {
    //        Text = "Get started ASAP!",
    //        CommentedAt = DateTime.Now.AddMinutes(-5),
    //        Task = mytasks[0],
    //        User = userlist[0]
    //    };

    //    var comment2 = new Comment
    //    {
    //        Text = "Don’t forget tests.",
    //        CommentedAt = DateTime.Now,
    //        Task = mytasks[0],
    //        User = userlist[1]
    //    };

    //    // Push into me's UnreadComments
    //    user.UnreadComments ??= new Stack<Comment>();
    //    user.UnreadComments.Push(comment1);
    //    user.UnreadComments.Push(comment2);
    //}
}
