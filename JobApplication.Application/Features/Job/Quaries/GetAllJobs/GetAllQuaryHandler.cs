using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Job.Quaries.GetAllJobs
{
    public class GetAllQuaryHandler : IRequestHandler<GetAllQuary, List<JobResponseDto>>
    {
        private readonly IJobRepository _jobs;

        public GetAllQuaryHandler(IJobRepository jobs)
        {
            _jobs = jobs;
        }

        public async Task<List<JobResponseDto>> Handle(GetAllQuary request, CancellationToken cancellationToken)
        {
            var query = _jobs.Get();
            if (request.isActive.HasValue) query = query.Where(j => j.IsActive == request.isActive);

            var jobs = await query.OrderByDescending(j => j.CreatedAt).ToListAsync();
            return jobs.Select(Map).ToList();
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
