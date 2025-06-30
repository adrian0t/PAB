using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.Text;
using TaskFlow.Application.Repositories;
using TaskFlow.Application.Services;
using TaskFlow.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TaskFlow.Infrastructure.Authentication;


namespace TaskFlow.Infrastructure.Services;

public class TokenCreator : ITokenCreator
{
    private readonly IConfiguration _config;
    private readonly IUserRepository _userRepository;
    private readonly ILoginHandler _loginHandler;

    public TokenCreator(
        IConfiguration config, 
        IUserRepository userRepository, 
        ILoginHandler loginHandler)
    {
        _userRepository = userRepository;
        _config = config;
        _loginHandler = loginHandler;
    }
    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user, string password)
    {
        if (user is null ||
            !await _userRepository.CheckPasswordAsync(user, password))
            return null;

        //var claims = new List<Claim>
        //{
        //    new Claim(ClaimTypes.NameIdentifier, user.UserName),
        //};

        //IList<string> roles = await _userRepository.GetRolesAsync(user);
        //claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var claims = await _loginHandler.AddClaimsAsync(user);

        var jwtCfg = _config.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtCfg["Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwtCfg["Issuer"],
            audience: jwtCfg["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds);

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenStr;
    }
    public async Task<ClaimsPrincipal> DecodeToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");

        return new ClaimsPrincipal(identity);
    }
}
