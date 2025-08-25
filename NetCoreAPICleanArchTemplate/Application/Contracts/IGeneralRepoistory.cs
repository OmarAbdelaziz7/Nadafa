using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IGeneralRepoistory<T> where T : class
    {
        public  Task<T> GetByIdAsync(int id);

        public  Task<List<T>> GetAllAsync();

        public  Task<T> AddAsync(T entity);

        public  Task<T> UpdateAsync(T entity);

        public  Task<bool> DeleteAsync(int id);

    }
}
