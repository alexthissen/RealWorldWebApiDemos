using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.HealthChecks;
using NSwag.Annotations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GameServerWebAPI.Controllers
{
    [SwaggerIgnore]
    public class HomeController : Controller
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            return new RedirectResult("~/swagger");
        }
    }
}
