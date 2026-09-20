using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobApplication.Application.Services;

public class JobService : IJobService
{
    private readonly IJobRepository _jobs;
    public JobService(IJobRepository jobs) => _jobs = jobs;

    public async Task<Result<JobResponseDto>> CreateAsync(int recruiterId ,CreateJobDto dto)
    {
        var job = new Job
        {
            Title = dto.Title,
            Description = dto.Description,
            RecruiterId = recruiterId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _jobs.InsertAsync(job);
        await _jobs.SaveChangesAsync();
        return Result<JobResponseDto>.Ok(Map(job));
    }

    public async Task<List<JobResponseDto>> GetAllAsync(bool? isActive)
    {
        var query = _jobs.Get();
        if (isActive.HasValue) query = query.Where(j => j.IsActive == isActive);

        var jobs = await query.OrderByDescending(j => j.CreatedAt).ToListAsync();
        return jobs.Select(Map).ToList();
    }

    public async Task<Result<JobResponseDto>> GetByIdAsync(int id)
    {
        var job = await _jobs.GetByIdAsync(id);
        return job is null
            ? Result<JobResponseDto>.Fail(ErrorType.NotFound, "Job not found.")
            : Result<JobResponseDto>.Ok(Map(job));
    }

    public async Task<Result<JobResponseDto>> UpdateAsync(int id,int recruiterId, UpdateJobDto dto)
    {
        var job = await _jobs.GetByIdAsync(id);
        if (job is null)
            return Result<JobResponseDto>.Fail(ErrorType.NotFound, "Job not found.");

        if (job.RecruiterId != recruiterId)
            return Result<JobResponseDto>.Fail(ErrorType.Forbidden, "You don't own this job.");

        if (!job.IsActive)
            return Result<JobResponseDto>.Fail(ErrorType.Validation, "A closed job can't be edited.");

        job.Title = dto.Title;
        job.Description = dto.Description;

        await _jobs.SaveChangesAsync();
        return Result<JobResponseDto>.Ok(Map(job));
    }

    public async Task<Result<JobResponseDto>> CloseAsync(int id, int recruiterId)
    {
        var job = await _jobs.GetByIdAsync(id);
        if (job is null)
            return Result<JobResponseDto>.Fail(ErrorType.NotFound, "Job not found.");

        if (job.RecruiterId != recruiterId)
            return Result<JobResponseDto>.Fail(ErrorType.Forbidden, "You don't own this job.");

        if (!job.IsActive)
            return Result<JobResponseDto>.Fail(ErrorType.Conflict, "Job is already closed.");

        job.IsActive = false;
        await _jobs.SaveChangesAsync();
        return Result<JobResponseDto>.Ok(Map(job));
    }

    private static JobResponseDto Map(Job j) => new()
    {
        Id = j.Id,
        Title = j.Title,
        Description = j.Description,
        IsActive = j.IsActive,
        RecruiterId = j.RecruiterId,
        CreatedAt = j.CreatedAt
    };
}