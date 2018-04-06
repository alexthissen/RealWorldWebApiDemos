using GameServerWebAPI.Model;
using GameServerWebAPI.Proxies;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameServerWebAPI.IntegrationTests
{
    internal class MockSteamClient : ISteamClient
    {
        public Task<ResponseWrapper> GetServerList(string key, int limit, string format = "json")
        {
            return Task.FromResult<ResponseWrapper>(
                new ResponseWrapper() {
                    Response = new ServerListResponse() {
                        Servers = new GameServerInfo[] {
                            new GameServerInfo() { Address = IPAddress.Loopback.ToString() }
                        }
                    }
                }
            );
        }
    }
}
