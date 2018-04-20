using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GameServerWebAPI.Infrastructure;
using GameServerWebAPI.Model;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Timeout;

namespace GameServerWebAPI.Controllers.V2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    [Produces("application/xml", "application/json")]
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
        [ProducesResponseType(typeof(IEnumerable<GameServerInfo>), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<IEnumerable<GameServerInfo>>> Get([FromQuery] int limit = 100)
        {
            ResponseWrapper serverList = null;

            try
            {
                logger.LogInformation("Acquiring server list with {SearchLimit} results.", limit);
                
                // Approach using LogMessage.Define 
                logger.GameServerListRequested(limit);

                serverList = await steamClient.GetServerList(steamOptions.Value.DeveloperApiKey, limit, steamOptions.Value.DefaultResponseFormat);
                
                // Processing and filtering of results
            }
            catch (HttpRequestException ex)
            {
                logger.LogWarning(ex, "Http request failed.");
                return StatusCode(StatusCodes.Status502BadGateway, "Failed request to external resource.");
            }
            catch (TimeoutRejectedException ex)
            {
                logger.LogWarning(ex, "Timeout occurred when retrieving server list for {SearchLimit} items.", limit);
                return StatusCode(StatusCodes.Status504GatewayTimeout, "Timeout on external web request.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unknown exception occurred while retrieving server list");

                // Exception shielding for all other exceptions
                return StatusCode(StatusCodes.Status500InternalServerError, "Request could not be processed.");
            }
            return Ok(serverList.Response.Servers);
        }
    }
}
