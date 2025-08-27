using Domain.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using Domain.Entities;

namespace Application.Contracts
{
    public interface IJwtTokenService
    {
         public Task<string> GenerateToken(ApplicationUser user);
         public Task<string> GenerateFactoryToken(Factory factory);
         public Task<ClaimsPrincipal> ValidateToken(string token);
    }
}
