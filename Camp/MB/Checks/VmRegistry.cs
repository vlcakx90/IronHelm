using Microsoft.Win32;
using System;

namespace MB1.Checks
{
    public class Vm_Registry : Check
    {
        public override string Name => nameof(Vm_Registry);

        public override bool Execute()
        {
            string[] paths = new string[]
            {
                // Vmware
                //"HKLM\\SYSTEM\\CurrentControlSet\\Enum\\PCI\\VEN_15AD*", // Subkey has the following structure: VEN_XXXX&DEV_YYYY&SUBSYS_ZZZZ&REV_WW
                "HKCU\\SOFTWARE\\VMware, Inc.\\VMware Tools",
                "HKLM\\SOFTWARE\\VMware, Inc.\\VMware Tools",
                "HKLM\\SYSTEM\\ControlSet001\\Services\vmdebug",
                "HKLM\\SYSTEM\\ControlSet001\\Services\vmmouse",
                "HKLM\\SYSTEM\\ControlSet001\\Services\\VMTools",
                "HKLM\\SYSTEM\\ControlSet001\\Services\\VMMEMCTL",
                "HKLM\\SYSTEM\\ControlSet001\\Services\vmware",
                "HKLM\\SYSTEM\\ControlSet001\\Services\vmci",
                "HKLM\\SYSTEM\\ControlSet001\\Services\vmx86",
                "HKLM\\SYSTEM\\CurrentControlSet\\Enum\\IDE\\CdRomNECVMWar_VMware_IDE_CD*",
                "HKLM\\SYSTEM\\CurrentControlSet\\Enum\\IDE\\CdRomNECVMWar_VMware_SATA_CD*",
                "HKLM\\SYSTEM\\CurrentControlSet\\Enum\\IDE\\DiskVMware_Virtual_IDE_Hard_Drive*",
                "HKLM\\SYSTEM\\CurrentControlSet\\Enum\\IDE\\DiskVMware_Virtual_SATA_Hard_Drive *",

                // Virtualbox
                //"HKLM\\SYSTEM\\CurrentControlSet\\Enum\\PCI\\VEN_80EE*", // Subkey has the following structure: VEN_XXXX & DEV_YYYY & SUBSYS_ZZZZ & REV_WW
                "HKLM\\HARDWARE\\ACPI\\DSDT\\VBOX__",
                "HKLM\\HARDWARE\\ACPI\\FADT\\VBOX__",
                "HKLM\\HARDWARE\\ACPI\\RSDT\\VBOX__",
                "HKLM\\SOFTWARE\\Oracle\\VirtualBox Guest Additions",
                "HKLM\\SYSTEM\\ControlSet001\\Services\\VBoxGuest",
                "HKLM\\SYSTEM\\ControlSet001\\Services\\VBoxMouse",
                "HKLM\\SYSTEM\\ControlSet001\\Services\\VBoxService",
                "HKLM\\SYSTEM\\ControlSet001\\Services\\VBoxSF",
                "HKLM\\SYSTEM\\ControlSet001\\Services\\VBoxVideo",
            };

            var registry = Registry.LocalMachine;

            foreach (var path in paths)
            {
                Console.WriteLine($"path: {path}");
                if (registry.GetValue(path, null) == null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
