
using DTOs;
using Microsoft.AspNetCore.Mvc;
using Models;
using PracticeApp.Middleware;
using Repository;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        public IAuthInterface _authInterface;
        public AuthController(IAuthInterface authInterface)
        {
            _authInterface = authInterface;
        }

        [HttpPost("signin")]
        public IActionResult SignIn(LoginDTO loginDto)
        {
            return Ok(_authInterface.SignIn(loginDto));
        }
        [Authorize]
        [HttpGet("privatedata")]
        public IActionResult GetData(string jwtToken)
        {
            return Ok(_authInterface.GetData(jwtToken));
        }
    }
}
