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

namespace JobApplication.Application.Features.Jobs.commend.ReviewApplication
{
    public class ReviewApplicationCommandHandler : IRequestHandler<ReviewApplicationCommand, Result<ApplicationResponseDto>>
    {
        private readonly IApplicationRepository _apps;
        private readonly IJobRepository _jobs;

        private static readonly Dictionary<JobApplicationStatus, JobApplicationStatus[]> AllowedTransitions = new()
        {
            [JobApplicationStatus.Applied] = new[] { JobApplicationStatus.UnderReview, JobApplicationStatus.Rejected },
            [JobApplicationStatus.UnderReview] = new[] { JobApplicationStatus.Interview, JobApplicationStatus.Rejected },
            [JobApplicationStatus.Interview] = new[] { JobApplicationStatus.Accepted, JobApplicationStatus.Rejected },
        };

        public ReviewApplicationCommandHandler(IApplicationRepository apps, IJobRepository jobs)
        {
            _apps = apps;
            _jobs = jobs;
        }

        public async Task<Result<ApplicationResponseDto>> Handle(ReviewApplicationCommand request, CancellationToken cancellationToken)
        {
            var app = await _apps.GetByIdAsync(request.id);
            if (app is null)
                return Result<ApplicationResponseDto>.Fail(ErrorType.NotFound, "Application not found.");

            if (app.Job.RecruiterId != request.recruiterId)
                return Result<ApplicationResponseDto>.Fail(ErrorType.Forbidden, "You don't own this job.");

            if (!AllowedTransitions.TryGetValue(app.JobApplicationStatus, out var allowed) ||
                !allowed.Contains(request.dto.NewStatus))
                return Result<ApplicationResponseDto>.Fail(ErrorType.Validation,
                    $"Invalid transition from {app.JobApplicationStatus} to {request.dto.NewStatus}.");

            app.JobApplicationStatus = request.dto.NewStatus;
            app.StatusUpdatedAt = DateTime.UtcNow;

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
