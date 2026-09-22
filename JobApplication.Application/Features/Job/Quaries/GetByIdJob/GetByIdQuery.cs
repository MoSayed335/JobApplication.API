using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Job.Quaries.GetByIdJob
{
    public class GetByIdQuery : IRequest<Result<JobResponseDto>>
    {
        public int id { get; set; }
    }
}
