using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Services;

public interface ITokenCreator
{
    public Task<string>? GenerateJwtTokenAsync(ApplicationUser user, string password);
    public Task<ClaimsPrincipal> DecodeToken(string token);
}