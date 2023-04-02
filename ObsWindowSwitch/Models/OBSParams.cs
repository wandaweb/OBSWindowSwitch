using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsWindowSwitch.Models
{
    public class OBSParams
    {
        public OBSParams(string websocketUrl, string password, string sceneName)
        {
            WebsocketUrl = websocketUrl;
            WebsocketPassword = password;
            SceneName = sceneName;
        }

        public OBSParams()
        {
            WebsocketUrl = "ws://localhost:4455";
            WebsocketPassword = "";
            SceneName = "Default";
        }

        public string WebsocketUrl { get; set; }
        public string SceneName { get; set; }
        public string WebsocketPassword { get; set; }
    }
}
