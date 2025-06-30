using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Infrastructure.Authentication;

public interface ILoginHandler
{
    public Task<IEnumerable<Claim>> AddClaimsAsync(ApplicationUser user);
}
