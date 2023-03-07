using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsWindowSwitch.Models
{
    public class SharedWindow
    {
        private string _obsSourceName, _title, _processName;
        private int _obsSource;
        private string _processId;
        private FindBy _findBy;
        private bool _shown = false;
        private bool _keepActive = false;
        private Guid _key;

        public enum FindBy { Title, ProcessName };

        public SharedWindow()
        {
            
        }

        public string ObsSourceName { get => _obsSourceName; set => _obsSourceName = value; }
        public string Title { get => _title; set => _title = value; }
        public string ProcessName { get => _processName; set => _processName = value; }
        public int ObsSource { get => _obsSource; set => _obsSource = value; }
        public FindBy SearchBy { get => _findBy; set => _findBy = value; }
        public bool Shown { get => _shown; set => _shown = value; }
        public bool KeepActive { get => _keepActive; set => _keepActive = value; }
        public Guid Key { get => _key; set => _key = value; }
    }
}
