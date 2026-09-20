using JobApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces.Auth
{
    public interface IJwtTokenGenerator
    {
        (string Token, DateTime ExpiresAt) Generate(User user);
    }
}
