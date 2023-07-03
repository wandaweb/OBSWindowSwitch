using OBSWebsocketDotNet;
using OBSWebsocketDotNet.Types;
using ObsWindowSwitch.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsWindowSwitch
{
    public class OBSConnection
    {
        private string _url, _password;
        private OBSWebsocket _obs;
        private bool _connected;
        private SharedWindow _initialWindow;
        private string _sceneName;

        public OBSConnection(string url= "ws://localhost:4455", string password="", string sceneName="Default")
        {
            _url = url;
            _password = password;
            _obs = new OBSWebsocket();
            _connected = false;
            _initialWindow = new SharedWindow { Title = ""};
            _sceneName = sceneName;
            Connect();
        }

        public void Connect()
        {
            _obs.ConnectAsync(_url, _password);
            Debug.WriteLine("Is connected? " + _obs.IsConnected);
            _obs.Connected += OnConnected;
        }

        private void OnConnected(Object obj, EventArgs e)
        {
            _connected = true;

            Debug.WriteLine("Socket connected");


            FetchSceneInfo();
        }

        public void FetchSceneInfo()
        {
            if (_obs.IsConnected)
            {
                Debug.WriteLine("Connected");

                // get active scene name
                _sceneName = _obs.GetCurrentProgramScene();
                Debug.WriteLine("Scene: " + _sceneName);

                // add initial scene items

                foreach (var win in StreamWindows.GetWindows())
                {
                    Debug.WriteLine(win.Value.Title);
                }

                foreach (var pair in StreamWindows.GetWindows())
                {
                    var win = pair.Value;
                    Debug.WriteLine("getting scene item id for " + win.ObsSourceName);
                    win.ObsSource = _obs.GetSceneItemId(_sceneName, win.ObsSourceName, 0);
                    Debug.WriteLine(win.ObsSource);
                    Debug.WriteLine(win.ObsSourceName, " id is ", win.ObsSource);
                }

                // switch to the foreground window
                if (_initialWindow.Title != "")
                    SwitchTo(_initialWindow);
            }
        }

        public List<string> GetAllSceneItems()
        {
            if (!_obs.IsConnected) return new List<string>();
            var sceneItems = _obs.GetSceneItemList(_sceneName);
            List<string> itemNames = new List<string>();
            foreach(var item in sceneItems)
            {
                itemNames.Add(item.SourceName);
            }
            return itemNames;
        }

        public void SwitchTo(SharedWindow window)
        {
            if (_obs.IsConnected)
            {
                Debug.WriteLine("window count: " +StreamWindows.GetWindows().Count);
                // hide other window items
                foreach (var pair in StreamWindows.GetWindows())
                {
                    var win = pair.Value;
                    Console.WriteLine(win.ObsSourceName);
                    if (win.KeepActive == false)
                    {
                        try
                        {
                            _obs.SetSceneItemEnabled(_sceneName, win.ObsSource, false);
                        } catch { }
                        Debug.WriteLine("hiding: " + win.ObsSourceName);
                        
                    }
                    else
                    {
                        Debug.WriteLine("not hiding " + win.ObsSourceName);
                    }
                    win.Shown = false;
                }
                // show the window item
                //if (window.Shown == false)
                //{
                    _obs.SetSceneItemEnabled(_sceneName, window.ObsSource, true);
                    window.Shown = true;
                    Debug.WriteLine("enabling: " + window.ObsSourceName);
                //}

            }
            else
            {
                // save this window so obs can switch
                // to it when the websocket connection
                // is established
                _initialWindow = window;
                Debug.WriteLine("not connected");
            }
        }
    }
}
