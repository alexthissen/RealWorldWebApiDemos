using GameServerWebAPI.Proxies;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GameServerWebAPI.IntegrationTests
{
    internal class MockSteamClient : ISteamClient
    {
        public Task<string> GetServerList(string key, int limit, string format = "json")
        {
            return Task.FromResult<string>(@"""mockgameserverlist""");
        }
    }
}
