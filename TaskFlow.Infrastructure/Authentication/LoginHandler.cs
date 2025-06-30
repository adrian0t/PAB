using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.Repositories;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Authentication;

public class LoginHandler : ILoginHandler
{
    private readonly IUserRepository _userRepository;
    public LoginHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<IEnumerable<Claim>> AddClaimsAsync(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
        };
        IEnumerable<string> roles = await _userRepository.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        return claims;
    }
}
