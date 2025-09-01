using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register/user")]
        public async Task<ActionResult<AuthResponseDto>> RegisterUser([FromBody] UserRegistrationDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterUserAsync(request);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }

        [HttpPost("register/factory")]
        public async Task<ActionResult<AuthResponseDto>> RegisterFactory([FromBody] FactoryRegistrationDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterFactoryAsync(request);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.LoginAsync(request);
            
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            
            return Unauthorized(result);
        }

        [HttpPost("validate-token")]
        [Authorize]
        public async Task<ActionResult<bool>> ValidateToken()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is required");
            }

            var isValid = await _authService.ValidateTokenAsync(token);
            return Ok(isValid);
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> GetCurrentUser()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            return Ok(new
            {
                UserId = User.FindFirst("nameidentifier")?.Value,
                Email = User.FindFirst("email")?.Value,
                Name = User.FindFirst("name")?.Value,
                Role = User.FindFirst("role")?.Value,
                Claims = claims
            });
        }
    }
}
