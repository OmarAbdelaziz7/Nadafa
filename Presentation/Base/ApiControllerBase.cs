using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Presentation.Base
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public abstract class ApiControllerBase : ControllerBase
    {
        protected int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Invalid user ID in token");
        }

        protected string GetCurrentUserEmail()
        {
            return User.FindFirst(ClaimTypes.Email)?.Value
                ?? throw new UnauthorizedAccessException("User email not found in token");
        }

        protected string GetCurrentUserRole()
        {
            return User.FindFirst(ClaimTypes.Role)?.Value
                ?? throw new UnauthorizedAccessException("User role not found in token");
        }

        protected bool IsUserInRole(string role)
        {
            return GetCurrentUserRole().Equals(role, StringComparison.OrdinalIgnoreCase);
        }

        protected ActionResult<T> HandleServiceResult<T>(T result, string errorMessage = "An error occurred")
        {
            if (result == null)
            {
                return NotFound(new { error = errorMessage });
            }

            return Ok(result);
        }

        protected ActionResult HandleServiceResult(bool success, string successMessage = "Operation completed successfully", string errorMessage = "An error occurred")
        {
            if (success)
            {
                return Ok(new { message = successMessage });
            }

            return BadRequest(new { error = errorMessage });
        }

        protected ActionResult HandleException(Exception ex, string userMessage = "An unexpected error occurred")
        {
            // Log the exception here if you have logging configured
            return StatusCode(500, new { error = userMessage });
        }
    }
}
