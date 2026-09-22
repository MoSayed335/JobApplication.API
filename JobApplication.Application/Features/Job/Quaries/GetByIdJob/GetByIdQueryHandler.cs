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

namespace JobApplication.Application.Features.Job.Quaries.GetByIdJob
{
    public class GetByIdQueryHandler : IRequestHandler<GetByIdQuery, Result<JobResponseDto>>
    {
        private readonly IJobRepository _job; 

        public GetByIdQueryHandler(IJobRepository job)
        {
            _job = job;
        }

        public async Task<Result<JobResponseDto>> Handle(GetByIdQuery request, CancellationToken cancellationToken)
        {
            var job = await _job.GetByIdAsync(request.id);
            return job is null
                ? Result<JobResponseDto>.Fail(ErrorType.NotFound, "Job not found.")
                : Result<JobResponseDto>.Ok(Map(job));
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
