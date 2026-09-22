using JobApplication.Application.DTOs;
using JobApplication.Domain.Enums;
using MediatR;

namespace JobApplication.Application.Features.Jobs.Quaries.GetAllQuary
{
    public class GetAllApplicationesQuary : IRequest<List<ApplicationResponseDto>>
    {
        //int? jobId, int? candidateId, int? recruiterId, JobApplicationStatus? status

        public int? JobId { get; set; }
        public int? CandidateId { get; set; }
        public int? RecruiterId { get; set; }
        public JobApplicationStatus? status { get; set; }
    }
}
