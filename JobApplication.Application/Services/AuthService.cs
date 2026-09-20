using JobApplication.Application.Common;
using JobApplication.Application.DTOs.Auth;
using JobApplication.Application.Interfaces.Auth;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _users;
        private readonly IJwtTokenGenerator _jwt;
        private readonly IPasswordHasher<User> _hasher;

        public AuthService(IUserRepository users, IJwtTokenGenerator jwt, IPasswordHasher<User> hasher)
        {
            _users = users; _jwt = jwt; _hasher = hasher;
        }

        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
        {
            var email = dto.Email.Trim().ToLowerInvariant();

            if (await _users.EmailExistsAsync(email))
                return Result<AuthResponseDto>.Fail(ErrorType.Conflict, "Email is already registered.");

            var user = new User { FullName = dto.FullName, Email = email, Role = dto.Role };
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            if (dto.Role == UserRole.Candidate)
            {
                var candidate = new Candidate { FullName = dto.FullName, Email = email }; // عدّل حسب الـ entity
                await _users.InsertCandidateAsync(candidate);
                await _users.SaveChangesAsync();
                user.ProfileId = candidate.Id;
            }

            await _users.InsertAsync(user);
            await _users.SaveChangesAsync();

            if (dto.Role == UserRole.Recruiter)
            {
                user.ProfileId = user.Id;
                await _users.SaveChangesAsync();
            }

            return Result<AuthResponseDto>.Ok(BuildResponse(user));
        }

        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            var user = await _users.GetByEmailAsync(dto.Email.Trim().ToLowerInvariant());

            if (user is null ||
                _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password) == PasswordVerificationResult.Failed)
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
