using JobApplication.Application.DTOs;
using MediatR;

namespace JobApplication.Application.Features.Job.Quaries.GetAllJobs
{
    public class GetAllQuary : IRequest<List<JobResponseDto>>
    {
        public bool? isActive { get; set; }
    }
}
