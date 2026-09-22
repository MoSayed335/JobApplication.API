using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Job.Command.CloseJob
{
    public class CloseJobCommand : IRequest<Result<JobResponseDto>>
    {
        public int Id { get; set; }
        public int recruiterId { get; set; }
    }
}
