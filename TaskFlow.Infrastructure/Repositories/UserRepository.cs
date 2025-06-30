using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.DTOs;
using TaskFlow.Application.Repositories;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Config;

namespace TaskFlow.Infrastructure.Repositories;
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username)
    {
        ApplicationUser? user = await _userManager.Users
            .Include(u => u.UserProjects)
            .Include(u => u.AssignedTasks)
            .Include(u => u.UnreadComments)
            .Include(u => u.Comments)
            .FirstOrDefaultAsync(u => u.UserName == username);

        return user;
    }

    public async Task<List<ApplicationUser>?> GetListByUsernameAsync(IEnumerable<string> usernames)
    {
        usernames ??= Enumerable.Empty<string>();

        List<ApplicationUser>? result = await _context.Users
            .Include(u => u.UserProjects)
            .Include(u => u.AssignedTasks)
            .Include(u => u.UnreadComments)
            .Include(u => u.Comments)
            .Where(u => usernames.Contains(u.UserName))
            .ToListAsync();

        return result;
    }

    //public async Task AddAsync(ApplicationUser user)
    //{
    //    await _context.AddAsync(user);
    //    await _context.SaveChangesAsync();
    //}
    public async Task<int> CountAsync()
    {
        return await _context.Users.CountAsync();
    }
    public async Task<bool> CheckPasswordAsync(ApplicationUser user, string password)
    {
        bool result = await _userManager.CheckPasswordAsync(user, password);
        return result;
    }
    public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
    {
        IList<string> result = await _userManager.GetRolesAsync(user);

        return result;
    }

    public async Task<List<ApplicationUser>> GetAllAsync()
    {
        List<ApplicationUser> result = await _userManager.Users
            .Include(u => u.UserProjects)
            .Include(u => u.AssignedTasks)
            .Include(u => u.UnreadComments)
            .Include(u => u.Comments)
            .ToListAsync();
        
        return result;
    }
    public async Task<bool> ExistsAsync(string username)
    {
        bool result = await _context.Users.AnyAsync(u => u.UserName == username);
        return result;
    }
    public async Task CreateAsync(LoginDTO loginDTO)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = loginDTO.userName,

        };
        var result = await _userManager.CreateAsync(user, loginDTO.password); 
        await _userManager.AddToRoleAsync(user, "User");
    }
}
