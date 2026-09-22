using JobApplication.Application.Common;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JobApplication.API.Controllers;

/// <summary>
/// Base API controller providing common profile claim resolution and domain error translation helpers.
/// </summary>
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Gets the profile ID associated with the current authenticated user identity.
    /// </summary>
    protected int ProfileId => int.Parse(User.FindFirstValue("profileId")!);

    /// <summary>
    /// Translates a failed application <see cref="Result{T}"/> into the corresponding HTTP error response.
    /// </summary>
    /// <typeparam name="T">The type of the underlying payload.</typeparam>
    /// <param name="r">The failed domain/application operation result.</param>
    /// <returns>An <see cref="IActionResult"/> with an appropriate HTTP status code (404, 403, 409, 401, or 400) and error payload.</returns>
    protected IActionResult ToError<T>(Result<T> r) => r.Error switch
    {
        ErrorType.NotFound => NotFound(new { error = r.Message }),
        ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { error = r.Message }),
        ErrorType.Conflict => Conflict(new { error = r.Message }),
        ErrorType.Unauthorized => Unauthorized(new { error = r.Message }),
        _ => BadRequest(new { error = r.Message })
    };
}