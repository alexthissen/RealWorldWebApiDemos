using GameServerWebAPI.ClientSdk;
using GameServerWebAPI.ClientSdk.Models;
using GameServerWebAPI.IntegrationTests;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GameServerWebAPI.V2.IntegrationTests
{
    [TestClass]
    public class ServiceContractIntegrationTests
    {
        TestServer testServer;
        HttpClient httpClient;
        DotNextAPI proxy;

        [TestInitialize]
        public void Initialize()
        {
            var builder = new WebHostBuilder()
                .UseEnvironment("Development")
                .UseStartup<Startup>()
                .ConfigureTestServices(services =>
                {
                    services.AddTransient<ISteamClient, MockSteamClient>();
                });

            // Create test stack
            testServer = new TestServer(builder);
            httpClient = testServer.CreateClient();
            proxy = new DotNextAPI(httpClient);
        }

        [TestMethod]
        public async Task OpenApiDocumentationAvailable()
        {
            // Act
            var response = await httpClient.GetAsync("/swagger/index.html?url=/swagger/v2/swagger.json");

            // Assert
            response.EnsureSuccessStatusCode();
            string responseHtml = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(responseHtml.Contains("swagger"));
        }

        [TestMethod]
        public async Task GetGameServerListV2_WithoutQueryString()
        {
            // Act
            var response = await proxy.GameServer.GetAsync();

            // Assert
            Assert.AreEqual(1, response.Count, "Should have received a single server in list");
            Assert.AreEqual(response[0].Addr, "127.0.0.1", "Should have received a single server in list");
        }

        [TestMethod]
        public async Task GetGameServerListV2_WithQueryString()
        {
            // Act
            var response = await proxy.GameServer.GetAsync(100);

            // Assert
            Assert.AreEqual(1, response.Count, "Should have received a single server in list");
            Assert.AreEqual(response[0].Addr, "127.0.0.1", "Should have received a single server in list");
        }

        [TestMethod]
        public async Task GetGameServerListV2Raw()
        {
            // Arrange 
            int limit = 1;

            // Act
            var result = await proxy.GameServer.GetWithHttpMessagesAsync(limit);

            // Assert
            Assert.IsNotNull(result, "Should have received a response.");
            Assert.AreEqual(HttpStatusCode.OK, result.Response.StatusCode, "Status code should be 200 OK");
            var response = await result.Response.Content.ReadAsAsync<IList<GameServerInfo>>();
            Assert.AreEqual(limit, response.Count, "Response body should contain one GameServerInfo");
            Assert.AreEqual("127.0.0.1", response[0].Addr, "Address of GameServerInfo should be loopback.");
        }
    }
}
