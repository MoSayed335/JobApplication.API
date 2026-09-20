// API/Controllers/JobsController.cs
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobApplication.API.Controllers;

[Route("api/[controller]")]
public class JobsController : ApiControllerBase
{
    private readonly IJobService _service;
    public JobsController(IJobService service) => _service = service;

    [HttpPost]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> Create(CreateJobDto dto)
    {
        var result = await _service.CreateAsync(ProfileId, dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value)
            : ToError(result);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
        => Ok(await _service.GetAllAsync(isActive));

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _service.GetByIdAsync(id);
        return result.IsSuccess ? Ok(result.Value) : ToError(result);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> Update(int id, UpdateJobDto dto)
    {
        var result = await _service.UpdateAsync(id, ProfileId, dto);
        return result.IsSuccess ? Ok(result.Value) : ToError(result);
    }

    [HttpPut("{id:int}/close")]
    [Authorize(Roles = "Recruiter")]
    public async Task<IActionResult> Close(int id)
    {
        var result = await _service.CloseAsync(id, ProfileId);
        return result.IsSuccess ? Ok(result.Value) : ToError(result);
    }
}