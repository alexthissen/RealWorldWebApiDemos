using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServerWebAPI.Model
{
    public class ResponseWrapper
    {
        [JsonProperty(PropertyName = "response")]
        public ServerListResponse Response { get; set; }
    }
}
