using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Contracts;

namespace Application.Implementations
{
    public class GeneralRepoistory
    {
        public Task<T> GetByIdAsync<T>(int id) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllAsync<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> AddAsync<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync<T>(int id) where T : class
        {
            throw new NotImplementedException();
        }




    }
}
