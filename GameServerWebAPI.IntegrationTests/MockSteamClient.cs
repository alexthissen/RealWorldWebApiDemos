using GameServerWebAPI.Model;
using GameServerWebAPI.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameServerWebAPI.IntegrationTests
{
    internal class MockSteamClient : ISteamClient
    {
        public Task<ResponseWrapper> GetServerList(string key, int limit, string format = "json")
        {
            var servers = new List<GameServerInfo>();
            for (int index = 0; index < limit; index++)
                servers.Add(new GameServerInfo() { Address = IPAddress.Loopback.ToString() });

            return Task.FromResult<ResponseWrapper>(
                new ResponseWrapper() {
                    Response = new ServerListResponse() {
                        Servers = servers.ToArray()
                    }
                });
        }
    }
}
