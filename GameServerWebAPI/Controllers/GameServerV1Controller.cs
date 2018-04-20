using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Refit;

namespace GameServerWebAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("0.9", Deprecated = true)]
    [AdvertiseApiVersions("2.0")]
    public class GameServerController : ControllerBase
    {
        public readonly IConfiguration Configuration;

        public GameServerController(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> Get([FromQuery] int limit = 100)
        {
            ISteamClient proxy = RestService.For<ISteamClient>(Configuration["SteamApiOptions:BaseUrl"]);

            return await proxy.GetServerList(Configuration["SteamApiOptions:DeveloperApiKey"], limit);
        }
    }

    [Headers("User-Agent: Steam WebAPI Client 1.0")]
    public interface ISteamClient
    {
        [Get("/GetServerList/v1")]
        Task<string> GetServerList(string key, int limit, string format = "json");
    }
    }
