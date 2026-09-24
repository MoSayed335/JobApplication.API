using JobApplication.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Infrastructure.Services
{
    /// <summary>
    /// Service responsible for recurring background cleanup of expired job postings.
    /// </summary>
    public class JobCleanupService : IJobCleanupService
    {
        private readonly IJobRepository _jobRepository;
        private readonly ILogger<JobCleanupService> _logger;
        private readonly int _expirationDays;

        public JobCleanupService(
            IJobRepository jobRepository,
            ILogger<JobCleanupService> logger,
            IConfiguration configuration)
        {
            _jobRepository = jobRepository;
            _logger = logger;
            _expirationDays = configuration.GetValue("Jobs:ExpirationDays", 30);
        }

        public async Task CloseExpiredJobsAsync(CancellationToken cancellationToken = default)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-_expirationDays);
            _logger.LogInformation("JobCleanupService: Starting recurring scan for active jobs created before {CutoffDate} (older than {Days} days)...",
                cutoffDate, _expirationDays);

            var expiredJobs = await _jobRepository.GetExpiredJobsAsync(cutoffDate, cancellationToken);

            if (expiredJobs.Count == 0)
            {
                _logger.LogInformation("JobCleanupService: Scan completed. No expired jobs found to close.");
                return;
            }

            foreach (var job in expiredJobs)
            {
                job.IsActive = false;
                _logger.LogInformation("JobCleanupService: Auto-closing expired Job #{JobId} ('{Title}', CreatedAt: {CreatedAt}).",
                    job.Id, job.Title, job.CreatedAt);
            }

            await _jobRepository.SaveChangesAsync();
            _logger.LogInformation("JobCleanupService: Successfully closed {Count} expired job(s).", expiredJobs.Count);
        }
    }
}
