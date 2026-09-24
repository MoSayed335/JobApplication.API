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

namespace JobApplication.Application.Features.Jobs.Quaries.GetAllQuary
{
    public class GetAllApplicationesQuaryHandler : IRequestHandler<GetAllApplicationesQuary, List<ApplicationResponseDto>>
    {
        private readonly IApplicationRepository _apps;

        public GetAllApplicationesQuaryHandler(IApplicationRepository apps)
        {
            _apps = apps;
        }
        public Task<List<ApplicationResponseDto>> Handle(GetAllApplicationesQuary request, CancellationToken cancellationToken)
        {
            var query = _apps.Get();

            if (request.JobId.HasValue) query = query.Where(a => a.JobId == request.JobId);
            if (request.CandidateId.HasValue) query = query.Where(a => a.CandidateId == request.CandidateId);
            if (request.RecruiterId.HasValue) query = query.Where(a => a.Job.RecruiterId == request.RecruiterId);
            if (request.status.HasValue) query = query.Where(a => a.JobApplicationStatus == request.status);

            return Task.FromResult(
                query.OrderByDescending(a => a.AppliedAt).AsEnumerable().Select(Map).ToList());
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
