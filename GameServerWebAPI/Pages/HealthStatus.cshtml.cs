using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameServerWebAPI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHealthCheck healthCheck;

        public IndexModel(IHealthCheck healthCheck)
        {
            this.healthCheck = healthCheck;
        }

        public TimeSpan ExecutionTime { get; private set; }
        public IHealthCheckResult HealthCheckResult { get; private set; }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public void OnGet()
        {
            var timedTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var stopwatch = Stopwatch.StartNew();
            var checkResult = healthCheck.CheckAsync(timedTokenSource.Token).GetAwaiter().GetResult();
            ExecutionTime = stopwatch.Elapsed;
            HealthCheckResult = checkResult;

        }
    }
}
