using JobApplication.Application.DTOs.Auth;
using JobApplication.Application.Features.Auth.Command.Login;
using JobApplication.Application.Features.Auth.Command.Register;
using JobApplication.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    /// <summary>
    /// Handles user authentication, registration, and JWT token issuance.
    /// </summary>
    [Tags("Authentication")]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class AuthController : ApiControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="auth">The authentication service application interface.</param>
        public AuthController(IAuthService auth, IMediator mediator)
        {
            _mediator = mediator;
            _auth = auth;
        }

        /// <summary>
        /// Registers a new user account as either a Candidate or Recruiter.
        /// </summary>
        /// <remarks>
        /// <b>Architecture Context:</b> Triggers the <c>RegisterAsync</c> Use Case via <see cref="IAuthService"/> (<c>JobApplication.Application.Services.IAuthService</c>).
        /// 
        /// Ensures email uniqueness, securely hashes the password using ASP.NET Core PasswordHasher, provisions the corresponding Candidate or Recruiter profile, and generates a JWT access token.
        /// </remarks>
        /// <param name="dto">The registration payload containing full name, email, password, and target role.</param>
        /// <returns>The authentication response containing the issued JWT token, expiration timestamp, role, and profile ID.</returns>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            //var result = await _auth.RegisterAsync(dto);
            var result = await _mediator.Send(new RegisterCommand() { dto =  dto });
            return result.IsSuccess ? StatusCode(StatusCodes.Status201Created, result.Value) : ToError(result);
        }

        /// <summary>
        /// Authenticates a user with email and password and generates a JWT access token.
        /// </summary>
        /// <remarks>
        /// <b>Architecture Context:</b> Triggers the <c>LoginAsync</c> Use Case via <see cref="IAuthService"/> (<c>JobApplication.Application.Services.IAuthService</c>).
        /// 
        /// Locates the user by email, verifies the password hash, and generates a signed JWT token with user claims (name identifier, role, and profileId).
        /// </remarks>
        /// <param name="dto">The login credentials including email address and plaintext password.</param>
        /// <returns>The authentication response containing the issued JWT token, expiration timestamp, role, and profile ID.</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            //var result = await _auth.LoginAsync(dto);
            var result = await _mediator.Send(new LoginCommand() { dto = dto });
            return result.IsSuccess ? Ok(result.Value) : ToError(result);
        }
    }
}
