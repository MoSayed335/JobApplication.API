using JobApplication.Application.DTOs;
using JobApplication.Application.Features.Job.Command.CloseJob;
using JobApplication.Application.Features.Job.Command.CreateJob;
using JobApplication.Application.Features.Job.Command.UpdateJob;
using JobApplication.Application.Features.Job.Quaries.GetAllJobs;
using JobApplication.Application.Features.Job.Quaries.GetByIdJob;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Services;
using JobApplication.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers;

/// <summary>
/// Manages job postings, queries, updates, and closure workflows.
/// </summary>
[Tags("Jobs")]
[Route("api/[controller]")]
public class JobsController : ApiControllerBase
{
    //private readonly IJobService _service;
    private readonly IMediator _mentor;

    /// <summary>
    /// Initializes a new instance of the <see cref="JobsController"/> class.
    /// </summary>
    /// <param name="service">The job domain service.</param>
    /// <param name="mediator">The MediatR mediator instance.</param>
    public JobsController(IJobService service , IMediator mediator)
    {
        //_service = service;
        _mentor = mediator;
    }

    /// <summary>
    /// Creates a new job posting (Recruiter only).
    /// </summary>
    /// <remarks>
    /// <b>Architecture Context:</b> Triggers the MediatR command <see cref="CreateJobCommand"/> (<c>JobApplication.Application.Features.Job.Command.CreateJob.CreateJobCommand</c>).
    /// 
    /// Associates the new job with the authenticated recruiter's profile ID, marks it active by default, and returns the created resource with a Location header pointing to <see cref="GetById"/>.
    /// </remarks>
    /// <param name="dto">The job details including title and description.</param>
    /// <returns>The created <see cref="JobResponseDto"/> with a 201 Created status and Location header.</returns>
    [HttpPost]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(JobResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create(CreateJobDto dto)
    {
        //var result = await _service.CreateAsync(ProfileId, dto);
        var result = await _mentor.Send(new CreateJobCommand() { recruiterId = ProfileId , dto = dto });
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : ToError(result);
    }

    /// <summary>
    /// Retrieves all job postings, optionally filtered by active status.
    /// </summary>
    /// <remarks>
    /// <b>Architecture Context:</b> Triggers the MediatR query <see cref="GetAllQuary"/> (<c>JobApplication.Application.Features.Job.Quaries.GetAllJobs.GetAllQuary</c>).
    /// 
    /// Fetches all jobs sorted by creation date descending. This endpoint is public and does not require authentication.
    /// </remarks>
    /// <param name="isActive">Optional filter: <c>true</c> for active jobs only, <c>false</c> for closed jobs, or <c>null</c> for all.</param>
    /// <returns>A collection of <see cref="JobResponseDto"/> items matching the filter.</returns>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<JobResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
        //=> Ok(await _service.GetAllAsync(isActive));
        => Ok(await _mentor.Send(new GetAllQuary() { isActive =  isActive }));

    /// <summary>
    /// Retrieves a specific job posting by its unique identifier.
    /// </summary>
    /// <remarks>
    /// <b>Architecture Context:</b> Triggers the <c>GetByIdAsync</c> Query / Use Case via <see cref="IJobService"/> (<c>JobApplication.Application.Interfaces.IJobService</c>).
    /// 
    /// Fetches the details of a single job. This endpoint is public and does not require authentication.
    /// </remarks>
    /// <param name="id">The unique identifier of the job.</param>
    /// <returns>The <see cref="JobResponseDto"/> if found.</returns>
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(JobResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        //var result = await _service.GetByIdAsync(id);
        var result = await _mentor.Send(new GetByIdQuery() { id = id });
        return result.IsSuccess ? Ok(result.Value) : ToError(result);
    }

    /// <summary>
    /// Updates an existing job posting (Recruiter only).
    /// </summary>
    /// <remarks>
    /// <b>Architecture Context:</b> Triggers the <c>UpdateAsync</c> Use Case via <see cref="IJobService"/> (<c>JobApplication.Application.Interfaces.IJobService</c>).
    /// 
    /// Verifies that the authenticated recruiter owns the job posting and that the job is active (closed jobs cannot be edited).
    /// </remarks>
    /// <param name="id">The unique identifier of the job to update.</param>
    /// <param name="dto">The updated title and description data.</param>
    /// <returns>The updated <see cref="JobResponseDto"/>.</returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(JobResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, UpdateJobDto dto)
    {
        //var result = await _service.UpdateAsync(id, ProfileId, dto);
        var result = await _mentor.Send(new UpdateJobCommand() { Id = id , recruiterId =ProfileId , dto = dto });
        return result.IsSuccess ? Ok(result.Value) : ToError(result);
    }

    /// <summary>
    /// Closes an active job posting to stop accepting further applications (Recruiter only).
    /// </summary>
    /// <remarks>
    /// <b>Architecture Context:</b> Triggers the <c>CloseAsync</c> Use Case via <see cref="IJobService"/> (<c>JobApplication.Application.Interfaces.IJobService</c>).
    /// 
    /// Verifies that the authenticated recruiter owns the job. Transitions <c>IsActive</c> to <c>false</c>. Fails with a conflict error if the job is already closed.
    /// </remarks>
    /// <param name="id">The unique identifier of the job to close.</param>
    /// <returns>The closed <see cref="JobResponseDto"/>.</returns>
    [HttpPut("{id:int}/close")]
    [Authorize(Roles = "Recruiter")]
    [ProducesResponseType(typeof(JobResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Close(int id)
    {
        //var result = await _service.CloseAsync(id, ProfileId);
        var result = await _mentor.Send(new CloseJobCommand() { Id =id , recruiterId = ProfileId});
        return result.IsSuccess ? Ok(result.Value) : ToError(result);
    }
}