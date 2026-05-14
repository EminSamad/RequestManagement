using Hangfire.Dashboard;

namespace RequestManagement.API.Filters;

public class AllowAllConnectionsFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context) => true;
}