using Application.Contracts;
using Application.DTOs.AuthDTOs;
using Domain.Entities;
using Application.Mapper;
using Microsoft.AspNetCore.Identity;
using Domain.Results;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepoistory _userRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUserRepoistory userRepo,
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService)
        {
            _userRepo = userRepo;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;

        }

        public async Task<Response<AuthResult>> RegisterUserAsync(RegisterUserDTO dto)
        {
            var user = AuthMapper.ToApplicationUserFromRegisterDTO(dto);
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return new Response<AuthResult>
                {
                    
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Succeeded = false,
                    Message = string.Join("; ", result.Errors.Select(e => e.Description)),
                    Data = null
                };
            }

            var token = await _jwtTokenService.GenerateToken(user);

            return new Response<AuthResult>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = "User registration successful",
                Data = new AuthResult
                {
                    UserId = user.Id,
                    IsAuthenticated = true,
                    Username = user.UserName,
                    Email = user.Email,
                    Token = token,
                    ExpiresOn = DateTime.UtcNow.AddHours(24)
                }
            };
        }
/*
        public async Task<Response<AuthResult>> RegisterFactoryAsync(RegisterFactoryDTO dto)
        {
            var factory = _authMapper.ToApplicationUserFromRegisterFactoryDTO(dto);
            var result = await _userManager.CreateAsync(factory, dto.Password);

            if (!result.Succeeded)
            {
                return new Response<AuthResult>
                {
                    Succeeded = false,
                    Message = string.Join("; ", result.Errors.Select(e => e.Description)),
                    Data = null
                };
            }

            var token = await _jwtTokenService.GenerateToken(factory);

            return new Response<AuthResult>
            {
                Succeeded = true,
                Message = "Factory registration successful",
                Data = new AuthResult
                {
                    UserId = int.Parse(factory.Id),
                    IsAuthenticated = true,
                    Username = factory.UserName,
                    Email = factory.Email,
                    Token = token,
                    ExpiresOn = DateTime.UtcNow.AddHours(24)
                }
            };
        }
*/
        public async Task<Response<AuthResult>> LoginAsync(LoginDTO dto)
        {
            var _appUser = AuthMapper.ToApplicationUserFromLoginDTO(dto);
            var user = await _userRepo.GetByEmailAsync(_appUser.Email!);

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return new Response<AuthResult>
                {
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Succeeded = false,
                    Message = "Invalid credentials",
                    Data = null
                };
            }

            var token = await _jwtTokenService.GenerateToken(user);

            return new Response<AuthResult>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = "Login successful",
                Data = new AuthResult
                {
                    UserId = user.Id,
                    IsAuthenticated = true,
                    Username = user.UserName,
                    Email = user.Email,
                    Token = token,
                    ExpiresOn = DateTime.UtcNow.AddHours(24)
                }
            };
        }
    }
}