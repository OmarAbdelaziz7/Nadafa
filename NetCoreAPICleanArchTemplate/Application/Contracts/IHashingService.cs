using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts
{
    public interface IHashingService<TEntity>
    {
        string HashPasswords(TEntity entity , string password);

        bool VerifyPassword(TEntity entity , string hashedPassword , string comingPassword);
    }
}
