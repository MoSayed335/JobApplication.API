using JobApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace JobApplication.Application.Interfaces
{
    public interface IJobRepository
    {
        Task InsertAsync(Job job);
        void Update(Job job);
        void Remove(Job job);
        IQueryable<Job> Get();
        Task<Job?> GetByIdAsync(int id);
        Task<List<Job>> GetExpiredJobsAsync(DateTime cutoffDate, CancellationToken cancellationToken = default);
        Task SaveChangesAsync();
    }
}
