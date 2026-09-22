using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Features.Jobs.commend.Canceledapplication;
using JobApplication.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JobApplication.Application.Features.Jobs.commend.CreateApplication;
using JobApplication.Application.Features.Jobs.Quaries.GetAllQuary;
using JobApplication.Application.Features.Jobs.commend.ReviewApplication;

namespace JobApplication.API.Controllers
{
    /// <summary>
    /// Manages candidate job applications, status reviews, and cancellations.
    /// </summary>
    [Tags("Applications")]
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationsController : ApiControllerBase
    {
        private readonly IApplicationService _service;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationsController"/> class.
        /// </summary>
        /// <param name="service">The application domain service.</param>
        /// <param name="mediator">The MediatR mediator instance.</param>
        public ApplicationsController(IApplicationService service, IMediator mediator) {
            _service = service;
            _mediator = mediator;
        }

        /// <summary>
        /// Submits a new job application for the authenticated candidate.
        /// </summary>
        /// <remarks>
        /// <b>Architecture Context:</b> Triggers the <c>CreateAsync</c> Use Case via <see cref="IApplicationService"/> (<c>JobApplication.Application.Interfaces.IApplicationService</c>).
        /// 
        /// Resolves the candidate's profile ID from user claims, validates that the target job exists and is actively accepting applications, verifies no prior application exists for this candidate/job combination, and creates the application with status 'Applied'.
        /// </remarks>
        /// <param name="dto">The application creation payload containing the target Job ID.</param>
        /// <returns>The created <see cref="ApplicationResponseDto"/> with a 201 Created status.</returns>
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Create(CreateApplicationDto dto)
        {
            //var result = await _service.CreateAsync(ProfileId, dto);
            var result = await _mediator.Send(new CreateApplicationCommand() { CandidateId = ProfileId , Dto =  dto });
            return result.IsSuccess
                ? StatusCode(StatusCodes.Status201Created, result.Value)
                : ToError(result);
        }

        /// <summary>
        /// Retrieves a filtered list of job applications based on the user's role and query parameters.
        /// </summary>
        /// <remarks>
        /// <b>Architecture Context:</b> Triggers the <c>GetAllAsync</c> Query / Use Case via <see cref="IApplicationService"/> (<c>JobApplication.Application.Interfaces.IApplicationService</c>).
        /// 
        /// Scopes results automatically based on caller identity:
        /// <list type="bullet">
        ///   <item><description><b>Candidate:</b> Scoped to applications submitted by the caller's profile ID.</description></item>
        ///   <item><description><b>Recruiter:</b> Scoped to applications for jobs posted by the caller's profile ID.</description></item>
        /// </list>
        /// Can be optionally filtered by job ID and application lifecycle status.
        /// </remarks>
        /// <param name="jobId">Optional filter for a specific job posting ID.</param>
        /// <param name="status">Optional filter for application lifecycle status (e.g., Applied, UnderReview, Interview, Accepted, Rejected, Cancelled).</param>
        /// <returns>A list of <see cref="ApplicationResponseDto"/> items matching the criteria.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ApplicationResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAll([FromQuery] int? jobId, [FromQuery] JobApplicationStatus? status)
        {
            //var apps = User.IsInRole("Candidate")
            //    ? await _service.GetAllAsync(jobId, ProfileId, null, status)
            //    : await _service.GetAllAsync(jobId, null, ProfileId, status);
            var apps = User.IsInRole("Candidate")
                ? await _mediator.Send(new GetAllApplicationesQuary() { JobId = jobId , CandidateId = ProfileId, RecruiterId =null , status = status})
                : await _mediator.Send(new GetAllApplicationesQuary() { JobId = jobId, CandidateId =null, RecruiterId = ProfileId, status = status });
            return Ok(apps);
        }

        /// <summary>
        /// Reviews an application and transitions its lifecycle status (Recruiter only).
        /// </summary>
        /// <remarks>
        /// <b>Architecture Context:</b> Triggers the <c>ReviewAsync</c> Use Case via <see cref="IApplicationService"/> (<c>JobApplication.Application.Interfaces.IApplicationService</c>).
        /// 
        /// Validates that the recruiter owns the job associated with the application, enforces valid state machine transitions (Applied -&gt; UnderReview/Rejected, UnderReview -&gt; Interview/Rejected, Interview -&gt; Accepted/Rejected), and updates the status.
        /// </remarks>
        /// <param name="id">The unique identifier of the job application to review.</param>
        /// <param name="dto">The review payload containing the target new status.</param>
        /// <returns>The updated <see cref="ApplicationResponseDto"/>.</returns>
        [HttpPut("{id:int}/review")]
        [Authorize(Roles = "Recruiter")]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Review(int id, ReviewApplicationDto dto)
        {
            //var result = await _service.ReviewAsync(id,ProfileId ,dto);
            var result = await _mediator.Send(new ReviewApplicationCommand() { id = id, recruiterId=ProfileId ,dto = dto });
            return result.IsSuccess ? Ok(result.Value) : ToError(result);
        }

        /// <summary>
        /// Cancels a submitted job application (Candidate only).
        /// </summary>
        /// <remarks>
        /// <b>Architecture Context:</b> Triggers the MediatR command <see cref="CanceledapplicationCommand"/> (<c>JobApplication.Application.Features.Jobs.commend.Canceledapplication.CanceledapplicationCommand</c>).
        /// 
        /// Verifies that the application belongs to the requesting candidate and that its status is currently cancellable ('Applied' or 'UnderReview'). Sets status to 'Cancelled' and records the cancellation timestamp.
        /// </remarks>
        /// <param name="id">The unique identifier of the job application to cancel.</param>
        /// <param name="candidateId">The candidate profile identifier requesting the cancellation.</param>
        /// <returns>The updated <see cref="ApplicationResponseDto"/> reflecting the cancelled state.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Candidate")]
        [ProducesResponseType(typeof(ApplicationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id, [FromQuery] int candidateId)
        {
            //var result = await _service.CancelAsync(id, candidateId);
            var result = await _mediator.Send(new CanceledapplicationCommand() { id = id , candidateId = candidateId });
            
            return result.IsSuccess ? Ok(result.Value) : ToError(result);
        }

        private new IActionResult ToError<T>(Result<T> r) => r.Error switch
        {
            ErrorType.NotFound => NotFound(new { error = r.Message }),
            ErrorType.Forbidden => StatusCode(StatusCodes.Status403Forbidden, new { error = r.Message }),
            ErrorType.Conflict => Conflict(new { error = r.Message }),
            _ => BadRequest(new { error = r.Message })
        };
    }
}
