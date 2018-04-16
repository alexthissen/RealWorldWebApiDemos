using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Mvc;

namespace GameServerWebAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("0.9", Deprecated = true)]
    [AdvertiseApiVersions("2.0")]
    public class GameServerController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public ActionResult<string> Get([FromQuery] int limit = 100)
        {
            var version = HttpContext.GetRequestedApiVersion();
            return Ok(version);
        }
    }
}
