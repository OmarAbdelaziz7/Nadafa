using Application.Contracts;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class FactoryRepository : GeneralRepoistory<Factory>, IFactoryRepoistory
    {
        public FactoryRepository(ApplicationDBContext context) : base(context) { }

        public async Task<Factory> GetByEmailAsync(string email)
        {
            return await _context.Set<Factory>().FirstOrDefaultAsync(f => f.Email == email);
        }
    }
}
