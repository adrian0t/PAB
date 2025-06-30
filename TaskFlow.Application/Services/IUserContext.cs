using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.DTOs;

namespace TaskFlow.Application.Services;

public interface IUserContext
{
    public Task<UserContextDTO>
        GetAuthorizations(ClaimsPrincipal user, string? projectKey);
}
