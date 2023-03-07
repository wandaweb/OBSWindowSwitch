using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ObsWindowSwitch
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        TextBox _websocketTextBox, _sceneNameTextBox;
        PasswordBox _passwordTextBox;

        public SettingsWindow()
        {
            InitializeComponent();
            _websocketTextBox = FindName("WebsocketText") as TextBox ?? new TextBox();
            _sceneNameTextBox = FindName("SceneNameText") as TextBox ?? new TextBox();
            _passwordTextBox = FindName("PasswordText") as PasswordBox ?? new PasswordBox();
            this.UpdateFields();
        }

        private void UpdateFields()
        {
            Debug.WriteLine("Update Fields");
            var settings = OBSSettings.GetSettings();
            Debug.WriteLine(settings.SceneName);
            _websocketTextBox.Text = settings.WebsocketUrl;
            _passwordTextBox.Password = settings.WebsocketPassword;
            _sceneNameTextBox.Text = settings.SceneName;
        }

        private void Save_Button_Click(object sender, RoutedEventArgs e)
        {
            string websocket = _websocketTextBox.Text;
            string sceneName = _sceneNameTextBox.Text;
            string password = _passwordTextBox.Password;
            var settings = OBSSettings.GetSettings();
            settings.WebsocketUrl = websocket;
            settings.SceneName = sceneName;
            settings.WebsocketPassword = password;
            ((MainWindow)Owner).UpdateOBSSettingsFile();
            Close();
        }
    }
}
