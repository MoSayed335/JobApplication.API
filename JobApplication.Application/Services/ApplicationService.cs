using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JobApplication.Application.Services
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _apps;
        private readonly IJobRepository _jobs;

        private static readonly Dictionary<JobApplicationStatus, JobApplicationStatus[]> AllowedTransitions = new()
        {
            [JobApplicationStatus.Applied] = new[] { JobApplicationStatus.UnderReview, JobApplicationStatus.Rejected },
            [JobApplicationStatus.UnderReview] = new[] { JobApplicationStatus.Interview, JobApplicationStatus.Rejected },
            [JobApplicationStatus.Interview] = new[] { JobApplicationStatus.Accepted, JobApplicationStatus.Rejected },
        };

        public ApplicationService(IApplicationRepository apps, IJobRepository jobs)
        {
            _apps = apps;
            _jobs = jobs;
        }

        // ---------- Create ----------
        public async Task<Result<ApplicationResponseDto>> CreateAsync(int candidateId,CreateApplicationDto dto)
        {
            var job = await _jobs.GetByIdAsync(dto.JobId);
            if (job is null)
                return Result<ApplicationResponseDto>.Fail(ErrorType.NotFound, "Job not found.");

            if (!job.IsActive)
                return Result<ApplicationResponseDto>.Fail(ErrorType.Validation, "This job is no longer accepting applications.");

            if (await _apps.ExistsAsync(candidateId, dto.JobId))
                return Result<ApplicationResponseDto>.Fail(ErrorType.Conflict, "You have already applied to this job.");

            var now = DateTime.UtcNow;
            var app = new JobCandidateApplication
            {
                CandidateId = candidateId,
                JobId = dto.JobId,
                JobApplicationStatus = JobApplicationStatus.Applied,
                AppliedAt = now,
                StatusUpdatedAt = now
            };

            await _apps.InsertAsync(app);
            await _apps.SaveChangesAsync();

            return Result<ApplicationResponseDto>.Ok(Map(app));
        }

        // ---------- Get All ----------
        public Task<List<ApplicationResponseDto>> GetAllAsync(int? jobId, int? candidateId, int? recruiterId, JobApplicationStatus? status)
        {
            var query = _apps.Get();

            if (jobId.HasValue) query = query.Where(a => a.JobId == jobId);
            if (candidateId.HasValue) query = query.Where(a => a.CandidateId == candidateId);
            if (recruiterId.HasValue) query = query.Where(a => a.Job.RecruiterId == recruiterId);
            if (status.HasValue) query = query.Where(a => a.JobApplicationStatus == status);

            return Task.FromResult(
                query.OrderByDescending(a => a.AppliedAt).AsEnumerable().Select(Map).ToList());
        }

        // ---------- Review (Recruiter) ----------
        public async Task<Result<ApplicationResponseDto>> ReviewAsync(int id, int recruiterId,ReviewApplicationDto dto)
        {
            var app = await _apps.GetByIdAsync(id);
            if (app is null)
                return Result<ApplicationResponseDto>.Fail(ErrorType.NotFound, "Application not found.");

            if (app.Job.RecruiterId != recruiterId)
                return Result<ApplicationResponseDto>.Fail(ErrorType.Forbidden, "You don't own this job.");

            if (!AllowedTransitions.TryGetValue(app.JobApplicationStatus, out var allowed) ||
                !allowed.Contains(dto.NewStatus))
                return Result<ApplicationResponseDto>.Fail(ErrorType.Validation,
                    $"Invalid transition from {app.JobApplicationStatus} to {dto.NewStatus}.");

            app.JobApplicationStatus = dto.NewStatus;
            app.StatusUpdatedAt = DateTime.UtcNow;

            _apps.Update(app);
            await _apps.SaveChangesAsync();

            return Result<ApplicationResponseDto>.Ok(Map(app));
        }

        // ---------- Cancel (Candidate) ----------
        public async Task<Result<ApplicationResponseDto>> CancelAsync(int id, int candidateId)
        {
            var app = await _apps.GetByIdAsync(id);
            if (app is null)
                return Result<ApplicationResponseDto>.Fail(ErrorType.NotFound, "Application not found.");

            if (app.CandidateId != candidateId)
                return Result<ApplicationResponseDto>.Fail(ErrorType.Forbidden, "This application doesn't belong to you.");

            if (app.JobApplicationStatus is not (JobApplicationStatus.Applied or JobApplicationStatus.UnderReview))
                return Result<ApplicationResponseDto>.Fail(ErrorType.Validation,
                    $"Cannot cancel an application in status {app.JobApplicationStatus}.");

            var now = DateTime.UtcNow;
            app.JobApplicationStatus = JobApplicationStatus.Cancelled;
            app.StatusUpdatedAt = now;
            app.CancelledAt = now;

            _apps.Update(app);
            await _apps.SaveChangesAsync();

            return Result<ApplicationResponseDto>.Ok(Map(app));
        }

        private static ApplicationResponseDto Map(JobCandidateApplication a) => new()
        {
            Id = a.Id,
            CandidateId = a.CandidateId,
            JobId = a.JobId,
            Status = a.JobApplicationStatus.ToString(),
            AppliedAt = a.AppliedAt,
            StatusUpdatedAt = a.StatusUpdatedAt,
            CancelledAt = a.CancelledAt
        };
    }
}
