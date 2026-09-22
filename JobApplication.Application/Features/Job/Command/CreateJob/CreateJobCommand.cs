using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Job.Command.CreateJob
{
    public class CreateJobCommand : IRequest<Result<JobResponseDto>>
    {
        public int recruiterId { get; set; }

        public CreateJobDto dto { get; set; }
    }
}
