using System.Linq;
using System.Threading.Tasks;
using Application.Contracts;
using Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Presentation.Base;

namespace Presentation.Controllers
{
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register/user")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> RegisterUser([FromBody] UserRegistrationDto request)
        {
            try
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
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Register a new factory
        /// </summary>
        [HttpPost("register/factory")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> RegisterFactory([FromBody] FactoryRegistrationDto request)
        {
            try
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
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Login user or factory
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto request)
        {
            try
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
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        [HttpPost("validate-token")]
        public async Task<ActionResult<bool>> ValidateToken()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Token is required");
                }

                var isValid = await _authService.ValidateTokenAsync(token);
                return Ok(isValid);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Get current user information
        /// </summary>
        [HttpGet("me")]
        public ActionResult<object> GetCurrentUser()
        {
            try
            {
                var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
                return Ok(new
                {
                    UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Email = User.FindFirst(ClaimTypes.Email)?.Value,
                    Name = User.FindFirst(ClaimTypes.Name)?.Value,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value,
                    Claims = claims
                });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Change user password
        /// </summary>
        [HttpPost("change-password")]
        public async Task<ActionResult<AuthResponseDto>> ChangePassword([FromBody] ChangePasswordDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var email = GetCurrentUserEmail();
                var result = await _authService.ChangePasswordAsync(email, request);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Sign out user
        /// </summary>
        [HttpPost("signout")]
        public async Task<ActionResult<SignOutResponseDto>> SignOutUser()
        {
            try
            {
                var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest("Token is required");
                }

                var result = await _authService.SignOutAsync(token);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("profile/user")]
        public async Task<ActionResult<AuthResponseDto>> UpdateUserProfile([FromBody] UpdateUserProfileDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentEmail = GetCurrentUserEmail();
                var result = await _authService.UpdateUserProfileAsync(currentEmail, request);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Update factory profile
        /// </summary>
        [HttpPut("profile/factory")]
        public async Task<ActionResult<AuthResponseDto>> UpdateFactoryProfile([FromBody] UpdateFactoryProfileDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentEmail = GetCurrentUserEmail();
                var result = await _authService.UpdateFactoryProfileAsync(currentEmail, request);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Delete user account
        /// </summary>
        [HttpDelete("account")]
        public async Task<ActionResult<AuthResponseDto>> DeleteAccount([FromBody] DeleteAccountDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var email = GetCurrentUserEmail();
                var result = await _authService.DeleteAccountAsync(email, request);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}
