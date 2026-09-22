using JobApplication.Application.Common;
using JobApplication.Application.DTOs.Auth;
using JobApplication.Application.Interfaces.Auth;
using JobApplication.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Auth.Command.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IPasswordHasher<User> _hasher;

        public LoginCommandHandler(IUserRepository users, IJwtTokenGenerator jwt, IPasswordHasher<User> hasher)
        {
            _users = users; _jwt = jwt; _hasher = hasher;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.GetByEmailAsync(request.dto.Email.Trim().ToLowerInvariant());

            if (user is null ||
                _hasher.VerifyHashedPassword(user, user.PasswordHash, request.dto.Password) == PasswordVerificationResult.Failed)
                return Result<AuthResponseDto>.Fail(ErrorType.Unauthorized, "Invalid email or password.");

            return Result<AuthResponseDto>.Ok(BuildResponse(user));
        }

        private AuthResponseDto BuildResponse(User user)
        {
            var (token, expires) = _jwt.Generate(user);
            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = expires,
                Role = user.Role.ToString(),
                ProfileId = user.ProfileId
            };
        }
    }
}
