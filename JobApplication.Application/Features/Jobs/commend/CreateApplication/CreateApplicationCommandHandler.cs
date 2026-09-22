using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.commend.CreateApplication
{
    public class CreateApplicationCommandHandler : IRequestHandler<CreateApplicationCommand, Result<ApplicationResponseDto>>
    {
        private readonly IApplicationRepository _apps;
        private readonly IJobRepository _jobs;

        private static readonly Dictionary<JobApplicationStatus, JobApplicationStatus[]> AllowedTransitions = new()
        {
            [JobApplicationStatus.Applied] = new[] { JobApplicationStatus.UnderReview, JobApplicationStatus.Rejected },
            [JobApplicationStatus.UnderReview] = new[] { JobApplicationStatus.Interview, JobApplicationStatus.Rejected },
            [JobApplicationStatus.Interview] = new[] { JobApplicationStatus.Accepted, JobApplicationStatus.Rejected },
        };

        public CreateApplicationCommandHandler(IApplicationRepository apps, IJobRepository jobs)
        {
            _apps = apps;
            _jobs = jobs;
        }
        public async Task<Result<ApplicationResponseDto>> Handle(CreateApplicationCommand request, CancellationToken cancellationToken)
        {
            var job = await _jobs.GetByIdAsync(request.Dto.JobId);
            if (job is null)
                return Result<ApplicationResponseDto>.Fail(ErrorType.NotFound, "Job not found.");

            if (!job.IsActive)
                return Result<ApplicationResponseDto>.Fail(ErrorType.Validation, "This job is no longer accepting applications.");

            if (await _apps.ExistsAsync(request.CandidateId, request.Dto.JobId))
                return Result<ApplicationResponseDto>.Fail(ErrorType.Conflict, "You have already applied to this job.");

            var now = DateTime.UtcNow;
            var app = new JobCandidateApplication
            {
                CandidateId = request.CandidateId,
                JobId = request.Dto.JobId,
                JobApplicationStatus = JobApplicationStatus.Applied,
                AppliedAt = now,
                StatusUpdatedAt = now
            };

            await _apps.InsertAsync(app);
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
