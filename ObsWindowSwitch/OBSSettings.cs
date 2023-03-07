using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObsWindowSwitch.Models;

namespace ObsWindowSwitch
{
    internal class OBSSettings
    {
        private static OBSSettings _instance;

        private OBSParams settings;

        public static OBSParams GetSettings()
        {
            if (_instance == null)
            {
                _instance = new OBSSettings();
                _instance.settings = new OBSParams();
            }
            return _instance.settings;
        }
    }
}
