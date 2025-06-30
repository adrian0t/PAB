using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Repositories;

public interface IUserRepository
{
    public Task<List<ApplicationUser>> GetAllAsync();
    public Task<ApplicationUser> GetByUsernameAsync(string username);
    public Task<List<ApplicationUser>> GetListByUsernameAsync(IEnumerable<string> usernames);
    public Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    //public Task AddAsync(ApplicationUser user);
    public Task<int> CountAsync();
    public Task<bool> ExistsAsync(string username);
    public Task<IList<string>> GetRolesAsync(ApplicationUser user);
    public Task CreateAsync(LoginDTO loginDTO);
}
