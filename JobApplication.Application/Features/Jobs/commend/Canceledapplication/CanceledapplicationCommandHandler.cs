using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.Jobs.commend.Canceledapplication
{
    public class CanceledapplicationCommandHandler
     : IRequestHandler<CanceledapplicationCommand, Result<ApplicationResponseDto>>
    {
        private readonly IApplicationRepository _apps;
        public CanceledapplicationCommandHandler(IApplicationRepository apps) { _apps = apps; }
        async Task<Result<ApplicationResponseDto>> IRequestHandler<CanceledapplicationCommand, Result<ApplicationResponseDto>>.Handle(CanceledapplicationCommand request, CancellationToken cancellationToken)
        {
            var app = await _apps.GetByIdAsync(request.id);
            if (app is null)
                return Result<ApplicationResponseDto>.Fail(ErrorType.NotFound, "Application not found.");

            if (app.CandidateId != request.candidateId)
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
