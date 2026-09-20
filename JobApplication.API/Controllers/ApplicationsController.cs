using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ApiControllerBase
    {
        private readonly IApplicationService _service;
        public ApplicationsController(IApplicationService service) => _service = service;

        [HttpPost]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status201Created)]
        [Authorize(Roles = "Candidate")]

        public async Task<IActionResult> Create(CreateApplicationDto dto)
        {

            var result = await _service.CreateAsync(ProfileId, dto);
            return result.IsSuccess
                ? StatusCode(StatusCodes.Status201Created, result.Value)
                : ToError(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int? jobId, [FromQuery] JobApplicationStatus? status)
        {
            var apps = User.IsInRole("Candidate")
                ? await _service.GetAllAsync(jobId, ProfileId, null, status)
                : await _service.GetAllAsync(jobId, null, ProfileId, status);
            return Ok(apps);
        }

        [HttpPut("{id:int}/review")]
        [Authorize(Roles = "Recruiter")]

        public async Task<IActionResult> Review(int id, ReviewApplicationDto dto)
        {
            var result = await _service.ReviewAsync(id,ProfileId ,dto);
            return result.IsSuccess ? Ok(result.Value) : ToError(result);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Candidate")]

        public async Task<IActionResult> Cancel(int id, [FromQuery] int candidateId)
        {
            var result = await _service.CancelAsync(id, candidateId);
            return result.IsSuccess ? Ok(result.Value) : ToError(result);
        }

        private IActionResult ToError<T>(Result<T> r) => r.Error switch
        {
            ErrorType.NotFound => NotFound(new { error = r.Message }),
            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { error = r.Message }),
            ErrorType.Conflict => Conflict(new { error = r.Message }),
            _ => BadRequest(new { error = r.Message })
        };
    }
}
