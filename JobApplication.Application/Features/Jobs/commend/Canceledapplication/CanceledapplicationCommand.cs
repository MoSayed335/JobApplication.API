using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.commend.Canceledapplication
{
    public class CanceledapplicationCommand : IRequest<Result<ApplicationResponseDto>>
    {
        public int id { get; set; }
        public int candidateId { get; set; }
    }
}
