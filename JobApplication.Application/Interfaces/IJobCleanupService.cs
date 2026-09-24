using System.Threading;
using System.Threading.Tasks;

namespace JobApplication.Application.Interfaces
{
    /// <summary>
    /// Background maintenance service for automated job lifecycle management.
    /// </summary>
    public interface IJobCleanupService
    {
        /// <summary>
        /// Scans active job postings and soft-closes any that have exceeded the retention threshold.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task representing the asynchronous cleanup operation.</returns>
        Task CloseExpiredJobsAsync(CancellationToken cancellationToken = default);
    }
}
