using Application.Contracts;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class UserRepoistory : GeneralRepoistory<ApplicationUser>, IUserRepoistory
    {
        public UserRepoistory(ApplicationDBContext context) : base(context)
        {
        }

        public async Task<ApplicationUser> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}