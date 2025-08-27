using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Base;
using Application.DTOs.AuthDTOs;
using Application.Contracts;
namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/user")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerDTO)
        {
            var result = await _authService.RegisterUserAsync(registerDTO);
            return NewResult(result);
        }

        [HttpPost("register/factory")]

        public async Task<IActionResult> RegisterFactory([FromBody] RegisterFactoryDTO registerDTO)
        {
            var result = await _authService.RegisterFactoryAsync(registerDTO);
            return NewResult(result);
        }

        [HttpPost("login/user")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var result = await _authService.LoginUserAsync(loginDTO);
            return NewResult(result);
        }

        [HttpPost("login/factory")]
        public async Task<IActionResult> LoginFactory([FromBody] LoginDTO loginDTO)
        {
            var result = await _authService.LoginFactoryAsync(loginDTO);
            return NewResult(result);
        }
    }
}
