using GameServerWebAPI.ClientSdk;
using GameServerWebAPI.IntegrationTests;
using GameServerWebAPI.Proxies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
