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
        private readonly IFactoryRepoistory _factoryRepoistory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IHashingService<Factory> _hashingService;

        public AuthService(
            IUserRepoistory userRepo,
            IFactoryRepoistory factoryRepoistory,
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IHashingService<Factory> hashingService)
        {
            _userRepo = userRepo;
            _factoryRepoistory = factoryRepoistory;
            _userManager = userManager;
            _jwtTokenService = jwtTokenService;
            _hashingService = hashingService;

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

        public async Task<Response<AuthResult>> LoginUserAsync(LoginDTO dto)
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

        public async Task<Response<AuthFactoryResponse>> RegisterFactoryAsync(RegisterFactoryDTO dto)
        {
           var factory = AuthMapper.ToFactoryFromRequestDTO(dto);

           // check if this Mail exists
           var existingFactory = await _factoryRepoistory.GetByEmailAsync(factory.Email);
           if (existingFactory != null)
           {
               return new Response<AuthFactoryResponse>
               {
                   StatusCode = System.Net.HttpStatusCode.Conflict,
                   Succeeded = false,
                   Message = "Email is already in use",
                   Data = null
               };
           }

              factory.HashedPassword = _hashingService.HashPasswords(factory, dto.Password);
              var result = await _factoryRepoistory.AddAsync(factory);
              var token = await _jwtTokenService.GenerateFactoryToken(factory);

           return new Response<AuthFactoryResponse>
           {
               StatusCode = System.Net.HttpStatusCode.OK,
               Succeeded = true,
               Message = "Factory registration successful",
               Data = new AuthFactoryResponse
               {
                   FactoryId = factory.Id.ToString(),
                   IsAuthenticated = true,
                   FactoryName = factory.FactoryName,
                   Email = factory.Email,
                   Token = token,
                   ExpiresOn = DateTime.UtcNow.AddHours(24)
               }
           };
        }

        public async Task<Response<AuthFactoryResponse>> LoginFactoryAsync(LoginDTO dto)
        {
            var factoryLogin = AuthMapper.ToFactoryFromLoginDTO(dto);
            var factory = await _factoryRepoistory.GetByEmailAsync(factoryLogin.Email!);

            if (factory == null || !_hashingService.VerifyPassword(factory, factory.HashedPassword, dto.Password))
            {
                return new Response<AuthFactoryResponse>
                {
                    StatusCode = System.Net.HttpStatusCode.Unauthorized,
                    Succeeded = false,
                    Message = "Invalid credentials",
                    Data = null
                };
            }

            var token = await _jwtTokenService.GenerateFactoryToken(factory);

            return new Response<AuthFactoryResponse>
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = "Login successful",
                Data = new AuthFactoryResponse
                {
                    FactoryId = factory.Id.ToString(),
                    IsAuthenticated = true,
                    FactoryName = factory.FactoryName,
                    Email = factory.Email,
                    Token = token,
                    ExpiresOn = DateTime.UtcNow.AddHours(24)
                }
            };
        }
    }
}