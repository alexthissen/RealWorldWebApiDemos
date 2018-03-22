using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GameServerWebAPI
{
    public class SteamApiOptions
    {
        public string DeveloperApiKey { get; set; }
        public string DefaultResponseFormat { get; set; }
        public string BaseUrl { get; set; }
    }
}
