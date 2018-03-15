using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.HealthChecks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameServerWebAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHealthCheckService healthCheck;

        public HomeController(IHealthCheckService healthCheck) => this.healthCheck = healthCheck;

        public async Task<IActionResult> HealthStatus()
        {
            var timedTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var stopwatch = Stopwatch.StartNew();
            var checkResult = await healthCheck.CheckHealthAsync(timedTokenSource.Token);
            ViewBag.ExecutionTime = stopwatch.Elapsed;
            return View(checkResult);
        }

        // GET: /<controller>/
        //[HttpGet]
        //public IActionResult Index()
        //{
        //    return new RedirectResult("~/swagger");
        //}
    }
}
