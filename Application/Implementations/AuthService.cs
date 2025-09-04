using Application.Contracts;
using Application.DTOs;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
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
        private readonly HashSet<string> _blacklistedTokens = new HashSet<string>();

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
                // Check if token is blacklisted
                if (_blacklistedTokens.Contains(token))
                {
                    return false;
                }

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

        public async Task<AuthResponseDto> ChangePasswordAsync(string email, ChangePasswordDto request)
        {
            // This is a simplified implementation
            // In a real application, you would validate against the database

            if (string.IsNullOrEmpty(email))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email is required"
                };
            }

            // For demo purposes, simulate password change
            // In real app, you would:
            // 1. Find user by email
            // 2. Verify current password using BCrypt.Verify
            // 3. Hash new password using BCrypt.HashPassword
            // 4. Update database

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Password changed successfully",
                Email = email,
                Role = "User" // This would come from the actual user
            };
        }

        public async Task<SignOutResponseDto> SignOutAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return new SignOutResponseDto
                {
                    IsSuccess = false,
                    Message = "Token is required"
                };
            }

            // Add token to blacklist
            _blacklistedTokens.Add(token);

            return new SignOutResponseDto
            {
                IsSuccess = true,
                Message = "Successfully signed out"
            };
        }

        public async Task<AuthResponseDto> UpdateUserProfileAsync(string currentEmail, UpdateUserProfileDto request)
        {
            // This is a simplified implementation
            // In a real application, you would validate against the database

            if (string.IsNullOrEmpty(currentEmail))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Current email is required"
                };
            }

            // For demo purposes, simulate profile update
            // In real app, you would:
            // 1. Find user by current email
            // 2. Check if new email is already taken (if email is changing)
            // 3. Update user information
            // 4. Generate new JWT token if email changed

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Profile updated successfully",
                Email = request.Email,
                Role = "User"
            };
        }

        public async Task<AuthResponseDto> UpdateFactoryProfileAsync(string currentEmail, UpdateFactoryProfileDto request)
        {
            // This is a simplified implementation
            // In a real application, you would validate against the database

            if (string.IsNullOrEmpty(currentEmail))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Current email is required"
                };
            }

            // For demo purposes, simulate profile update
            // In real app, you would:
            // 1. Find factory by current email
            // 2. Check if new email is already taken (if email is changing)
            // 3. Update factory information
            // 4. Generate new JWT token if email changed

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Profile updated successfully",
                Email = request.Email,
                Role = "Factory"
            };
        }

        public async Task<AuthResponseDto> DeleteAccountAsync(string email, DeleteAccountDto request)
        {
            // This is a simplified implementation
            // In a real application, you would validate against the database

            if (string.IsNullOrEmpty(email))
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Email is required"
                };
            }

            if (!request.ConfirmDeletion)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "You must confirm account deletion"
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = "Password confirmation does not match"
                };
            }

            // For demo purposes, simulate account deletion
            // In real app, you would:
            // 1. Find user/factory by email
            // 2. Verify password using BCrypt.Verify
            // 3. Check for any pending transactions or data dependencies
            // 4. Soft delete or permanently delete the account
            // 5. Clean up related data (pickup requests, marketplace items, etc.)
            // 6. Invalidate any active sessions/tokens

            return new AuthResponseDto
            {
                IsSuccess = true,
                Message = "Account deleted successfully",
                Email = email,
                Role = "User" // This would come from the actual user/factory
            };
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
