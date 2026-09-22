using JobApplication.Application.Common;
using JobApplication.Application.DTOs.Auth;
using JobApplication.Application.Interfaces.Auth;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Features.Auth.Command.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IPasswordHasher<User> _hasher;

        public RegisterCommandHandler(IUserRepository users, IJwtTokenGenerator jwt, IPasswordHasher<User> hasher)
        {
            _users = users; _jwt = jwt; _hasher = hasher;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var email = request.dto.Email.Trim().ToLowerInvariant();

            if (await _users.EmailExistsAsync(email))
                return Result<AuthResponseDto>.Fail(ErrorType.Conflict, "Email is already registered.");

            var user = new User { FullName = request.dto.FullName, Email = email, Role = request.dto.Role };
            user.PasswordHash = _hasher.HashPassword(user, request.dto.Password);

            if (request.dto.Role == UserRole.Candidate)
            {
                var candidate = new Candidate { FullName = request.dto.FullName, Email = email };
                await _users.InsertCandidateAsync(candidate);
                await _users.SaveChangesAsync();
                user.ProfileId = candidate.Id;
            }

            await _users.InsertAsync(user);
            await _users.SaveChangesAsync();

            if (request.dto.Role == UserRole.Recruiter)
            {
                user.ProfileId = user.Id;
                await _users.SaveChangesAsync();
            }

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
