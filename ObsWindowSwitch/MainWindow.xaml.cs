using OBSWebsocketDotNet;
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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Text.Json;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using System.Collections.Concurrent;

namespace ObsWindowSwitch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string CONFIG_FILE = "myWindows.json";
        public static string OBS_SETTINGS_FILE = "myObsSettings.json";
        private TextBlock _infoText;
        private StackPanel _stackPanel;
        private OBSConnection _connection;
        
        private WindowTracker _windowTracker;

        private delegate void OneArgDelegate(String arg);

        public MainWindow()
        {
            this.InitializeComponent();
            _infoText = FindName("Info") as TextBlock ?? new TextBlock();
            _stackPanel = FindName("StackPanel") as StackPanel ?? new StackPanel();
            
            LoadWindowSources();
            RefreshWindowSources();
            LoadOBSSettings();

            var settings = OBSSettings.GetSettings();
            _connection = new OBSConnection(settings.WebsocketUrl, settings.WebsocketPassword);

        }

        private void LoadWindowSources()
        {
            string text = File.ReadAllText(CONFIG_FILE);
            var windows = JsonSerializer.Deserialize<ConcurrentDictionary<Guid, SharedWindow>>(text);
            StreamWindows.SetWindows(windows);
        }

        private void LoadOBSSettings()
        {
            string text = File.ReadAllText(OBS_SETTINGS_FILE);
            OBSParams settings = JsonSerializer.Deserialize<OBSParams>(text);
            OBSSettings.GetSettings().WebsocketUrl = settings.WebsocketUrl;
            OBSSettings.GetSettings().WebsocketPassword = settings.WebsocketPassword;
            OBSSettings.GetSettings().SceneName = settings.SceneName;
        }

        public void RefreshWindowSources()
        {
            _stackPanel.Children.Clear();
            foreach (var pair in StreamWindows.GetWindows())
            {
                SharedWindow win = pair.Value;
                StackPanel itemPanel = new StackPanel();
                itemPanel.Orientation = Orientation.Horizontal;
                Label l = new Label();
                l.Content = win.Title;
                itemPanel.Children.Add(l);

                Button edit = new Button();
                edit.Content = "Edit";

                edit.Click += (sender, e) => {
                    EditWindowSource(win);
                };
                itemPanel.Children.Add(edit);

                Button del = new Button();
                del.Content = "Delete";
                del.Click += (sender, e) =>
                {
                    DeleteWindowSource(win);
                };
                itemPanel.Children.Add(del);

                _stackPanel.Children.Add(itemPanel);
            }            
        }

        private void DeleteWindowSource(SharedWindow win)
        {
            _infoText.Text = "Deleting " + win.Title + " Key: " + win.Key;
            SharedWindow deletedWin;
            bool success = StreamWindows.GetWindows().Remove(win.Key, out deletedWin);
            Debug.WriteLine("Deleting window ", success.ToString());
            UpdateConfigFile();
            RefreshWindowSources();
        }

        public void UpdateConfigFile()
        {
            var json = JsonSerializer.Serialize(StreamWindows.GetWindows());
            File.WriteAllTextAsync("myWindows.json", json);
        }

        public void UpdateOBSSettingsFile()
        {
            var json = JsonSerializer.Serialize(OBSSettings.GetSettings());
            File.WriteAllTextAsync("myObsSettings.json", json);
        }

        private void EditWindowSource(SharedWindow win)
        {
            _infoText.Text = win.Title;
            EditWindowSourceWindow editWin = new EditWindowSourceWindow(_connection);
            editWin.Win = win;
            editWin.Owner = this;
            editWin.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            _windowTracker = new WindowTracker(_connection);
            _infoText.Text = "Clicked";
            _windowTracker.StartWindowCheckingTimer();


            var processList = (new ProcessFinder()).ListAllProcesses();
            var processListText = JsonSerializer.Serialize(processList);
            _infoText.Text = processListText;
        }

        private void UpdateInfoBox(String text)
        {
            _infoText.Text = text;
        }

        private void Add_Window_Button_Click(object sender, RoutedEventArgs e)
        {
            EditWindowSourceWindow addWindow = new EditWindowSourceWindow(_connection);
            addWindow.Owner = this;
            addWindow.Show();
        }

        private void Settings_Button_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.Show();

        }
    }
}
