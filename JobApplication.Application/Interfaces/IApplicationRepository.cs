using JobApplication.Domain.Entities;

namespace JobApplication.Application.Interfaces
{
    public interface IApplicationRepository
    {
        Task InsertAsync(JobCandidateApplication application);
        void Update(JobCandidateApplication application);
        void Remove(JobCandidateApplication application);
        IQueryable<JobCandidateApplication> Get();               
        Task<JobCandidateApplication?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int candidateId, int jobId);
        Task SaveChangesAsync();
    }
}
