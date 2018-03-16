using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Mvc;

namespace GameServerWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameServerController : ControllerBase
    {
        private readonly ISteamClient steamClient;

        public GameServerController(ISteamClient client)
        {
            this.steamClient = client;
        }

        // GET api/values
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            string servers = null;
            try
            {
                // TODO: Use Azure Key Vault for secrets
                servers = await steamClient.GetServerList("ACCFDF4D1ACA9775A74119BF71609F6D", 10000, "xml");

                // TODO: Processing and filtering of results
            }
            catch (Exception ex) {
                return BadRequest("Timeout");
                // TODO: Proper logging strategy
            }
            return Ok(servers);
        }
    }
}
