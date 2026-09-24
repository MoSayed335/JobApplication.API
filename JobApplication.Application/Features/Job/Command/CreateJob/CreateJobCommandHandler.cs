using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using MediatR;

namespace JobApplication.Application.Features.Job.Command.CreateJob
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, Result<JobResponseDto>>
    {
        private readonly IJobRepository _jobs;
        public CreateJobCommandHandler(IJobRepository jobs) => _jobs = jobs;
        public async Task<Result<JobResponseDto>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            var job = new JobApplication.Domain.Entities.Job
            {
                Title = request.dto.Title,
                Description = request.dto.Description,
                RecruiterId = request.recruiterId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _jobs.InsertAsync(job);
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
