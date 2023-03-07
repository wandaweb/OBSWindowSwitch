using ObsWindowSwitch.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsWindowSwitch
{
    public class StreamWindows
    {
        private static ConcurrentDictionary<Guid, SharedWindow>? _windowsDictionary;

        public static void SetWindows(ConcurrentDictionary<Guid, SharedWindow> myWindows)
        {
            _windowsDictionary = myWindows;
        }

        public static ConcurrentDictionary<Guid, SharedWindow> GetWindows()
        {
            if (_windowsDictionary == null)
            {
                _windowsDictionary = new ConcurrentDictionary<Guid, SharedWindow>();
            }
            return _windowsDictionary;
        }
    }
}
