using ObsWindowSwitch.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ObsWindowSwitch
{
    public class WindowTracker
    {
        private OBSConnection _connection;

        public WindowTracker(OBSConnection obsConnection)
        {
            _connection = obsConnection;
        }

        public void StartWindowCheckingTimer()
        {
            Timer timer = new Timer(1500);
            timer.Elapsed += OnTimedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            IntPtr handle = GetForegroundWindow();

            Debug.WriteLine(" -------------------------- ");

            int count = 256;
            StringBuilder sb = new StringBuilder(count);
            int length = GetWindowText(handle, sb, count);
            if (length > 0)
            {
                Debug.WriteLine("Window Title: ", sb.ToString());
                /*_infoText.Dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Normal,
                new OneArgDelegate(UpdateInfoBox),
                sb.ToString());*/
            }

            uint processId;
            uint pid = GetWindowThreadProcessId(handle, out processId);
            Debug.WriteLine(processId);

            Process process = null;
            
            try
            {
                process = Process.GetProcessById((int)processId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            if (process != null)
            {
                Debug.WriteLine("Process name: " + process.ProcessName);

                foreach(var pair in StreamWindows.GetWindows())
                {
                    var win = pair.Value;
                    Debug.WriteLine("Checking window: " + win.Title);
                    if (win.SearchBy == SharedWindow.FindBy.ProcessName
                        && process.ProcessName == win.ProcessName)
                    {
                        Debug.WriteLine("Enabling scene item: " + win.ProcessName + " - " + win.ObsSource);
                        if (!win.Shown)
                        {
                            _connection.SwitchTo(win);
                        }
                    }
                    
                }
            }
            
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr handle, StringBuilder sb, int count);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr handle, out uint lpdwProcessId);
    }
}
