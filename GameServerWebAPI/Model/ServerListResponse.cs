using Newtonsoft.Json;

namespace GameServerWebAPI.Model
{
    public class ServerListResponse
    {
        [JsonProperty(PropertyName = "servers")]
        public GameServerInfo[] Servers { get; set; }
    }
}
