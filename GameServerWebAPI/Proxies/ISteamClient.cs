using GameServerWebAPI.Model;
using Refit;
using System.Threading.Tasks;

namespace GameServerWebAPI.Proxies
{
    [Headers("User-Agent: Steam WebAPI Client 2.0")]
    public interface ISteamClient
    {
        [Get("/GetServerList/v1")]
        Task<ResponseWrapper> GetServerList(string key, int limit, string format = "json");
    }
}
