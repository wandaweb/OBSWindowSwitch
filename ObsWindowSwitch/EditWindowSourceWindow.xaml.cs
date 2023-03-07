using ObsWindowSwitch.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for EditWindowSource.xaml
    /// </summary>
    /// 

    public partial class EditWindowSourceWindow : Window
    {
        private SharedWindow? win;
        private TextBox _processName, _obsName;
        private ListBox _processListBox, _obsSourceListBox;
        private CheckBox _checkBox;
        private ProcessFinder _processFinder;
        private List<string> _processList, _sceneItemList;
        private OBSConnection _obs;
        private Grid _grid;

        public EditWindowSourceWindow(OBSConnection obs)
        {
            _obs = obs;
            InitializeComponent();
            _processName = FindName("ProcessName") as TextBox ?? new TextBox();
            _obsName = FindName("OBSName") as TextBox ?? new TextBox();
            _checkBox = FindName("AlwaysVisible") as CheckBox ?? new CheckBox();
            _processListBox = FindName("ProcessListBox") as ListBox ?? new ListBox();
            _obsSourceListBox = FindName("ObsSourceListBox") as ListBox ?? new ListBox();
            _processFinder = new ProcessFinder();
            _processListBox.LostFocus += _processListBox_LostFocus;
            _processListBox.SelectionChanged += _processListBox_SelectionChanged;
            _obsSourceListBox.LostFocus += _obsSourceListBox_LostFocus;
            _obsSourceListBox.SelectionChanged += _obsSourceListBox_SelectionChanged;
            _processList = _processFinder.ListAllProcesses();
            _sceneItemList = _obs.GetAllSceneItems();
            _grid = FindName("Grid") as Grid ?? new Grid();
            win = null;
            this.Title = "New Source";
        }

        private void _obsSourceListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = _obsSourceListBox.SelectedItem as string;
            if(_obsSourceListBox.SelectedItem != null)
            {
                _obsName.Text = selectedItem;
                _obsSourceListBox.Visibility = Visibility.Hidden;
            }
        }

        private void _obsSourceListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _obsSourceListBox.Visibility = Visibility.Hidden;
        }

        private void _processListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedItem = _processListBox.SelectedItem as string;
            Debug.WriteLine("Selected: ", selectedItem);
            if (_processListBox.SelectedItem != null)
            {
                _processName.Text = selectedItem;
                _processListBox.Visibility = Visibility.Hidden;
            }
        }

        private void _processListBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _processListBox.Visibility = Visibility.Hidden;
            Debug.WriteLine("Process list box lost focus");
        }

        public SharedWindow Win
        {
            get => win; 
            set
            {
                win = value;
                UpdateFields();
                this.Title = "Edit Source";
            }
        }

        private void UpdateFields()
        {
            _processName.Text = win.ProcessName;
            _obsName.Text = win.ObsSourceName;
            _checkBox.IsChecked = win.KeepActive;
            _obsSourceListBox.Visibility = Visibility.Hidden;
            _processListBox.Visibility = Visibility.Hidden;
        }

        private void ProcessName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_processListBox != null)
            {
                _processList = _processFinder.ListAllProcesses();

            }
        }

        private void OBSName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_obsName != null)
            {
                string text = _obsName.Text;
                if (text.Length > 2)
                {
                    _obsSourceListBox.Visibility = Visibility.Visible;
                    _obsSourceListBox.Items.Clear();
                    foreach(var item in _sceneItemList)
                    {
                        if (item.StartsWith(text))
                        {
                            _obsSourceListBox.Items.Add(item);
                        }
                    }
                }
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _grid.Focus();
            Debug.WriteLine("Mouse down on window");
            _processListBox.Visibility = Visibility.Hidden;
            _obsSourceListBox.Visibility = Visibility.Hidden;
        }

        private void OBSName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (_obsSourceListBox != null)
            {
                _sceneItemList = _obs.GetAllSceneItems();

            }
        }

        private void ProcessName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_processName != null)
            {
                string text = _processName.Text;
                if (text.Length > 2)
                {
                    _processListBox.Visibility = Visibility.Visible;
                    _processListBox.Items.Clear();
                    foreach(var process in _processList)
                    {
                        if(process.StartsWith(text))
                        {
                            _processListBox.Items.Add(process);

                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string title = _processName.Text;

            string obsName = _obsName.Text;

            bool alwaysVisible = _checkBox.IsChecked ?? false;

            if (win != null)
            {
                win.Title = title;
                win.ProcessName = title;
                win.ObsSourceName = obsName;
                win.KeepActive = alwaysVisible;
            }
            else
            {
                SharedWindow newWindow = new SharedWindow
                {
                    SearchBy = SharedWindow.FindBy.ProcessName,
                    Title = title,
                    ProcessName = title,
                    ObsSourceName = obsName,
                    Shown = false,
                    KeepActive = alwaysVisible,
                    Key = Guid.NewGuid()
                };
                StreamWindows.GetWindows().TryAdd(newWindow.Key, newWindow); 
            }
            ((MainWindow)Owner).UpdateConfigFile();
            ((MainWindow)Owner).RefreshWindowSources();

            Close();
        }
    }
}
