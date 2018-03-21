using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Mvc;

namespace GameServerWebAPI.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("0.9", Deprecated = true)]
    [AdvertiseApiVersions("2.0")]
    public class GameServerController : ControllerBase
    {
        private readonly ISteamClient steamClient;

        public GameServerController(ISteamClient client)
        {
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
            try
            {
                // TODO: Use Azure Key Vault for secrets
                servers = await steamClient.GetServerList("ACCFDF4D1ACA9775A74119BF71609F6D", limit, "xml");

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
