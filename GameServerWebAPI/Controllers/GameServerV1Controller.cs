using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace GameServerWebAPI.Controllers.V1
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [ApiVersion("0.9", Deprecated = true)]
    [AdvertiseApiVersions("2.0")]
    public class GameServerController : ControllerBase
    {
        private readonly IOptionsSnapshot<SteamApiOptions> steamOptions;
        private readonly ISteamClient steamClient;

        public GameServerController(ISteamClient client, IOptionsSnapshot<SteamApiOptions> options)
        {
            this.steamOptions = options;
            this.steamClient = client;
        }

        // GET api/gameserver
        /// <summary>
        /// Retrieve a list of online game servers.
        /// </summary>
        /// <param name="limit">Maximum number of servers to retrieve.</param>
        /// <returns>List of online game servers.</returns>
        /// <response code="200">The list was successfully retrieved.</response>
        /// <response code="400">The request parameters were invalid or a timeout while retrieving list occurred.</response>
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<string>> Get([FromQuery] int limit = 100)
        {
            string servers = null;
            //HttpContext.Request.Headers["acc"]
            try
            {
                // TODO: Use Azure Key Vault for secrets
                servers = await steamClient.GetServerList(steamOptions.Value.DeveloperApiKey, limit, steamOptions.Value.DefaultResponseFormat);

                // TODO: Processing and filtering of results
            }
            catch (Exception) {
                return BadRequest("Timeout");
                // TODO: Proper exception handling and logging strategy
            }
            return Ok(servers);
        }
    }
}
