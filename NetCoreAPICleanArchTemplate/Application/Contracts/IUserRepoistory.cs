using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Application.Contracts;

namespace Application.Contracts
{
    public interface IUserRepoistory : IGeneralRepoistory<Domain.Entities.ApplicationUser>
    {
        public Task<ApplicationUser> GetByEmailAsync(string email);

        //public Task<bool> UpdatePassword(int userId, string newPassword);
    }
}
