using Hangfire.Dashboard;

namespace JobApplication.API
{
    /// <summary>
    /// Authorization filter allowing access to the Hangfire Dashboard in local development and demonstration environments.
    /// </summary>
    public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // In development and demonstration environments, permit access.
            // In production, enforce role-based authentication or IP whitelisting.
            return true;
        }
    }
}
