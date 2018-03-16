using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServerWebAPI.Proxies
{
    [Headers("User-Agent: Steam WebAPI Client 1.0")]
    public interface ISteamClient
    {
        [Get("/GetServerList/v1")]
        Task<string> GetServerList(string key, int limit, string format = "json");
    }
}
