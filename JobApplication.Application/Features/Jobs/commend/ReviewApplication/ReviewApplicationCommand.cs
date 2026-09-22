using JobApplication.Application.Common;
using JobApplication.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Jobs.commend.ReviewApplication
{
    public class ReviewApplicationCommand : IRequest<Result<ApplicationResponseDto>>
    {
        public int id { get; set; }
        public int recruiterId { get; set; }
        public ReviewApplicationDto dto { get; set; }

    }
}
