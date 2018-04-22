using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Refit;
using System.Threading.Tasks;

namespace GameServerWebAPI.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameServerController : ControllerBase
    {
        public readonly IConfiguration Configuration;

        public GameServerController(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<string>> Get([FromQuery] int limit = 100)
        {
            ISteamClient proxy = RestService.For<ISteamClient>(Configuration["SteamBaseUrl"]);

            return await proxy.GetServerList(Configuration["SteamApiKey"], limit);
        }
    }

    [Headers("User-Agent: Steam WebAPI Client 1.0")]
    public interface ISteamClient
    {
        [Get("/GetServerList/v1")]
        Task<string> GetServerList(string key, int limit, string format = "json");
    }
}
