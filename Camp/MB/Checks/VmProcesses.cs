using System.Collections.Generic;
using System.Diagnostics;

namespace MB1.Checks
{
    public class Vm_Processes : Check
    {
        public override string Name => nameof(Vm_Processes);

        public override bool Execute()
        {
            List<string> processes = new List<string>
            {
                // Vmware
                "vmtoolsd",
                "vmacthlp",
                "vmwaretray",
                "vmwareuser",
                "vmware",
                "vmount2",

                // Virtualbox
                "vboxservice",
                "vboxtray",
            };

            var ps = Process.GetProcesses();

            foreach (var p in ps)
            {
                if (processes.Contains(p.ProcessName))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
