using Newtonsoft.Json;

namespace GameServerWebAPI.Model
{
    public class GameServerInfo
    {
        [JsonProperty(PropertyName = "addr")]
        public string Address { get; set; }
        [JsonProperty(PropertyName = "gameport")]
        public int GamePort { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "appid")]
        public int ApplicationId { get; set; }
        [JsonProperty(PropertyName = "gamedir")]
        public string GameDirectory { get; set; }

        public string steamid { get; set; }
        public string version { get; set; }
        public string product { get; set; }
        public int region { get; set; }
        public int players { get; set; }
        public int max_players { get; set; }
        public int bots { get; set; }
        public string map { get; set; }
        public bool secure { get; set; }
        public bool dedicated { get; set; }
        public string os { get; set; }
        public string gametype { get; set; }
    }
}
