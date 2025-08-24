using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Base;
using Application.DTOs.AuthDTOs;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : AppControllerBase
    {
        [HttpPost("register/user")]
        public IActionResult Register([FromBody] RegisterUserDTO registerDTO)
        {
            // call the service to register the user
            // return the result
            return Ok(registerDTO);
            
        }

        [HttpPost("register/factory")]

        public IActionResult RegisterFactory([FromBody] RegisterFactoryDTO registerDTO)
        {
            // call the service to register the factory
            // return the result
            return Ok(registerDTO);
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            // call the service to login the user or factory
            // return the result
            return Ok(loginDTO);
        }
    }
}
