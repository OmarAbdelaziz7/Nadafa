using System;
using Application.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Services
{
    public class HashingService<TEntity> : IHashingService<TEntity> where TEntity : class 
    {
        private readonly PasswordHasher<TEntity> _hasher = new();

        public string HashPasswords(TEntity entity, string password)
        {
            return _hasher.HashPassword(entity, password);
        }

        public bool VerifyPassword(TEntity entity, string hashedPassword, string providedPassword)
        {
            var result = _hasher.VerifyHashedPassword(entity, hashedPassword, providedPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
