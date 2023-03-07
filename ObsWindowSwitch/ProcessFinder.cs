using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObsWindowSwitch
{
    internal class ProcessFinder
    {
        public List<string> ListAllProcesses()
        {
            List<string> result = new List<string>();
            Process[] processCollection = Process.GetProcesses();
            foreach (Process p in processCollection)
            {
                result.Add(p.ProcessName);
            }
            return result.Distinct<string>().ToList();
        }
    }
}
