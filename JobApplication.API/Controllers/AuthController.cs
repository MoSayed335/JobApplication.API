using JobApplication.Application.DTOs.Auth;
using JobApplication.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    // API/Controllers/AuthController.cs
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _auth;
        public AuthController(IAuthService auth) => _auth = auth;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var result = await _auth.RegisterAsync(dto);
            return result.IsSuccess ? StatusCode(StatusCodes.Status201Created, result.Value) : ToError(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var result = await _auth.LoginAsync(dto);
            return result.IsSuccess ? Ok(result.Value) : ToError(result);
        }
    }
}
