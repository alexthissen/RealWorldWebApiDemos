using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Timeout;

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
        private readonly ILogger<GameServerController> logger;
        private readonly ISteamClient steamClient;

        public GameServerController(ISteamClient client, IOptionsSnapshot<SteamApiOptions> options, ILoggerFactory logger)
        {
            this.steamOptions = options;
            this.logger = logger.CreateLogger<GameServerController>();
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
            catch (HttpRequestException ex)
            {
                // TODO: Structured logging
                logger.LogWarning(ex, "Http request failed.");
                return StatusCode(StatusCodes.Status502BadGateway, "Failed request to external resource.");
            }
            catch (TimeoutRejectedException ex)
            {
                logger.LogWarning(ex, "Timeout occurred when retrieving server list");
                return StatusCode(StatusCodes.Status504GatewayTimeout, "Timeout on external web request.");
            }
            catch (Exception ex)
            {
                // TODO: Proper exception handling and logging strategy
                logger.LogError(ex, "Unknown exception occurred while retrieving server list");

                // Exception shielding for all other exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, "Request could not be processed.");
            }
            return Ok(servers);
        }
    }
}
