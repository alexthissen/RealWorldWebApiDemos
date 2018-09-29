using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace GameServerWebAPI.ClientSdk
{
    public partial class SteamServerBrowserWebAPI
    {
        public SteamServerBrowserWebAPI(HttpClient client): base(client, false)
        {
            Initialize();
            BaseUri = client.BaseAddress;
        }
    }
}
