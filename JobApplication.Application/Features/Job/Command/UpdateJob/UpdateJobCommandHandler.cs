using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Job.Command.UpdateJob
{
    public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, Result<JobResponseDto>>
    {
        private readonly IJobRepository _jobs;
        public UpdateJobCommandHandler(IJobRepository jobs)
        {
            _jobs = jobs;
        }

        public async Task<Result<JobResponseDto>> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            var job = await _jobs.GetByIdAsync(request.Id);
            if (job is null)
                return Result<JobResponseDto>.Fail(ErrorType.NotFound, "Job not found.");

            if (job.RecruiterId != request.recruiterId)
                return Result<JobResponseDto>.Fail(ErrorType.Forbidden, "You don't own this job.");

            if (!job.IsActive)
                return Result<JobResponseDto>.Fail(ErrorType.Validation, "A closed job can't be edited.");

            job.Title = request.dto.Title;
            job.Description = request.dto.Description;

            await _jobs.SaveChangesAsync();
            return Result<JobResponseDto>.Ok(Map(job));
        }

        private static JobResponseDto Map(JobApplication.Domain.Entities.Job j) => new()
        {
            Id = j.Id,
            Title = j.Title,
            Description = j.Description,
            IsActive = j.IsActive,
            RecruiterId = j.RecruiterId,
            CreatedAt = j.CreatedAt
        };
    }
}
