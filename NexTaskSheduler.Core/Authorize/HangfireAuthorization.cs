using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace NexTaskScheduler.Core.Authorize
{
    public class HangfireAuthorization : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context) => true;
    }
}