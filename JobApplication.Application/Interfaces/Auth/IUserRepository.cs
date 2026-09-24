using JobApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces.Auth
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task InsertAsync(User user);
        Task InsertCandidateAsync(Candidate candidate);
        Task InsertRecruiterAsync(Recruiter recruiter);
        Task SaveChangesAsync();
    }
}
