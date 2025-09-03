using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<AuthResponseDto> RegisterUserAsync(UserRegistrationDto request)
        {
            // This is a simplified implementation
            // In a real application, you would inject a repository or unit of work

            // Generate JWT token for demo purposes
            var user = new User
            {
                Id = 1, // This would come from the database
                Name = request.Name,
                Email = request.Email,
                Address = request.Address,
                Age = request.Age,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = Role.User,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "User registered successfully",
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<AuthResponseDto> RegisterFactoryAsync(FactoryRegistrationDto request)
        {
            // This is a simplified implementation
            // In a real application, you would inject a repository or unit of work

            var factory = new Factory
            {
                Id = 1, // This would come from the database
                Name = request.Name,
                Email = request.Email,
                Address = request.Address,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                PhoneNumber = request.PhoneNumber,
                BusinessLicense = request.BusinessLicense,
                Role = Role.Factory,
                CreatedAt = DateTime.UtcNow,
                IsActive = true,
                IsVerified = false
            };

            var token = GenerateJwtToken(factory);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Factory registered successfully",
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                Email = factory.Email,
                Role = factory.Role.ToString()
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto request)
        {
            // This is a simplified implementation
            // In a real application, you would validate against the database

            // For demo purposes, accept any login with valid email format
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Invalid email or password"
                };
            }

            // Create a mock user for demo
            var user = new User
            {
                Id = 1,
                Name = "Demo User",
                Email = request.Email,
                Role = Role.User
            };

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Login successful",
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
                Email = user.Email,
                Role = user.Role.ToString()
            };
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:ExpirationInMinutes"])),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateJwtToken(Factory factory)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:SecretKey"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, factory.Id.ToString()),
                    new Claim(ClaimTypes.Email, factory.Email),
                    new Claim(ClaimTypes.Name, factory.Name),
                    new Claim(ClaimTypes.Role, factory.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_configuration["JwtSettings:ExpirationInMinutes"])),
                Issuer = _configuration["JwtSettings:Issuer"],
                Audience = _configuration["JwtSettings:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
