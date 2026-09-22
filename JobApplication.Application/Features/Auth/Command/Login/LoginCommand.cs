using JobApplication.Application.Common;
using JobApplication.Application.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Auth.Command.Login
{
    public class LoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public LoginDto dto { get; set; }
    }
}
