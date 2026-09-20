using JobApplication.Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobApplication.API.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected int ProfileId => int.Parse(User.FindFirstValue("profileId")!);
    protected IActionResult ToError<T>(Result<T> r) => r.Error switch
    {
        ErrorType.NotFound => NotFound(new { error = r.Message }),
        ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { error = r.Message }),
        ErrorType.Conflict => Conflict(new { error = r.Message }),
        ErrorType.Unauthorized => Unauthorized(new { error = r.Message }),
        _ => BadRequest(new { error = r.Message })
    };
}