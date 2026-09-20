using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Domain.Enums;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobApplication.Infrastructure.Repositories
{
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly ApplicationDbContext _context;
        public ApplicationRepository(ApplicationDbContext context) => _context = context;

        public async Task InsertAsync(JobCandidateApplication application)
            => await _context.JobCandidateApplications.AddAsync(application);

        public void Update(JobCandidateApplication application)
            => _context.JobCandidateApplications.Update(application);

        public void Remove(JobCandidateApplication application)
            => _context.JobCandidateApplications.Remove(application);

        public IQueryable<JobCandidateApplication> Get()
            => _context.JobCandidateApplications.AsNoTracking();

        public Task<JobCandidateApplication?> GetByIdAsync(int id)
            => _context.JobCandidateApplications.Include(a => a.Job)
                       .FirstOrDefaultAsync(a => a.Id == id);

        public Task<bool> ExistsAsync(int candidateId, int jobId)
            => _context.JobCandidateApplications.AnyAsync(a =>
                   a.CandidateId == candidateId &&
                   a.JobId == jobId &&
                   a.JobApplicationStatus != JobApplicationStatus.Cancelled);

        public Task SaveChangesAsync() => _context.SaveChangesAsync();
    }
}
