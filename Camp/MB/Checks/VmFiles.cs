using System.IO;

namespace MB1.Checks
{
    public class Vm_Files : Check
    {
        public override string Name => nameof(Vm_Files);

        public override bool Execute()
        {
            string[] files = new string[]
            { 
                // Vmware
                "c:\\windows\\system32\\drivers\\vmmouse.sys",
                "c:\\windows\\system32\\drivers\\vmnet.sys",
                "c:\\windows\\system32\\drivers\\vmxnet.sys",
                "c:\\windows\\system32\\drivers\\vmhgfs.sys",
                "c:\\windows\\system32\\drivers\\vmx86.sys",
                "c:\\windows\\system32\\drivers\\hgfs.sys",

                // Virtualbox
                "c:\\windows\\system32\\drivers\\VBoxMouse.sys",
                "c:\\windows\\system32\\drivers\\VBoxGuest.sys",
                "c:\\windows\\system32\\drivers\\VBoxSF.sys",
                "c:\\windows\\system32\\drivers\\VBoxVideo.sys",
                "c:\\windows\\system32\\vboxdisp.dll",
                "c:\\windows\\system32\\vboxhook.dll",
                "c:\\windows\\system32\\vboxmrxnp.dll",
                "c:\\windows\\system32\\vboxogl.dll",
                "c:\\windows\\system32\\vboxoglarrayspu.dll",
                "c:\\windows\\system32\\vboxoglcrutil.dll",
                "c:\\windows\\system32\\vboxoglerrorspu.dll",
                "c:\\windows\\system32\\vboxoglfeedbackspu.dll",
                "c:\\windows\\system32\\vboxoglpackspu.dll",
                "c:\\windows\\system32\\vboxoglpassthroughspu.dll",
                "c:\\windows\\system32\\vboxservice.exe",
                "c:\\windows\\system32\\vboxtray.exe",
                "c:\\windows\\system32\\VBoxControl.exe",
            };

            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
