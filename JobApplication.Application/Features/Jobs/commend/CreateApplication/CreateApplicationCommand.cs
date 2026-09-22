using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.commend.CreateApplication
{
    public class CreateApplicationCommand : IRequest<Result<ApplicationResponseDto>>
    {
        public int CandidateId { get; set; }
        public CreateApplicationDto Dto { get; set; }
    }
}
