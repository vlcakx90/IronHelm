using System;
using System.Runtime.InteropServices;

namespace SharedArsenal.Native
{
    public static class Advapi
    {
        [DllImport("advapi32.dll")]
        public static extern bool LogonUserA(
            string lpszUsername,
            string lpszDomain,
            string lpszPassword,
            LogonProvider dwLogonType,
            LogonUserProvider dwLogonProvider,
            ref IntPtr phToken
            );

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool ImpersonateLoggedOnUser(IntPtr hToken);

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool OpenProcessToken(IntPtr ProcessHandle, DesiredAccess DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public extern static bool DuplicateTokenEx(
            IntPtr hExistingToken,
            TokenAccess dwDesiredAccess,
            ref SECURITY_ATTRIBUTES lpTokenAttributes,
            SecurityImpersonationLevel ImpersonationLevel,
            TokenType TokenType,
            out IntPtr phNewToken);

        // https://github.com/dahall/Vanara/blob/c12f2495b2e1d6168a181aa5ea2793f0f927a502/PInvoke/Security/AdvApi32/SecurityBaseApi.cs
        public enum LogonProvider
        {
            /// <summary>
            /// This logon type is intended for users who will be interactively using the computer, such as a user being logged on by a
            /// terminal server, remote shell, or similar process. This logon type has the additional expense of caching logon information
            /// for disconnected operations; therefore, it is inappropriate for some client/server applications, such as a mail server.
            /// </summary>
            LOGON32_LOGON_INTERACTIVE = 2,

            /// <summary>
            /// This logon type is intended for high performance servers to authenticate plaintext passwords. The LogonUser function does not
            /// cache credentials for this logon type.
            /// </summary>
            LOGON32_LOGON_NETWORK = 3,

            /// <summary>
            /// This logon type is intended for batch servers, where processes may be executing on behalf of a user without their direct
            /// intervention. This type is also for higher performance servers that process many plaintext authentication attempts at a time,
            /// such as mail or web servers.
            /// </summary>
            LOGON32_LOGON_BATCH = 4,

            /// <summary>Indicates a service-type logon. The account provided must have the service privilege enabled.</summary>
            LOGON32_LOGON_SERVICE = 5,

            /// <summary>
            /// GINAs are no longer supported.
            /// <para>
            /// <c>Windows Server 2003 and Windows XP:</c> This logon type is for GINA DLLs that log on users who will be interactively using
            /// the computer. This logon type can generate a unique audit record that shows when the workstation was unlocked.
            /// </para>
            /// </summary>
            LOGON32_LOGON_UNLOCK = 7,

            /// <summary>
            /// This logon type preserves the name and password in the authentication package, which allows the server to make connections to
            /// other network servers while impersonating the client. A server can accept plain-text credentials from a client, call
            /// LogonUser, verify that the user can access the system across the network, and still communicate with other servers.
            /// </summary>
            LOGON32_LOGON_NETWORK_CLEARTEXT = 8,

            /// <summary>
            /// This logon type allows the caller to clone its current token and specify new credentials for outbound connections. The new
            /// logon session has the same local identifier but uses different credentials for other network connections. This logon type is
            /// supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
            /// </summary>
            LOGON32_LOGON_NEW_CREDENTIALS = 9
        }

        public enum LogonUserProvider
        {
            /// <summary>
            /// Use the standard logon provider for the system. The default security provider is negotiate, unless you pass NULL for the
            /// domain name and the user name is not in UPN format. In this case, the default provider is NTLM.
            /// </summary>
            LOGON32_PROVIDER_DEFAULT = 0,

            /// <summary>Use the Windows NT 3.5 logon provider.</summary>
            LOGON32_PROVIDER_WINNT35 = 1,

            /// <summary>Use the NTLM logon provider.</summary>
            LOGON32_PROVIDER_WINNT40 = 2,

            /// <summary>Use the negotiate logon provider.</summary>
            LOGON32_PROVIDER_WINNT50 = 3,

            /// <summary>Use the virtual logon provider.</summary>
            LOGON32_PROVIDER_VIRTUAL = 4
        }

        public enum DesiredAccess
        {
            STANDARD_RIGHTS_REQUIRED = 0x000F0000,
            STANDARD_RIGHTS_READ = 0x00020000,
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_READ = (STANDARD_RIGHTS_READ | TOKEN_QUERY),

            TOKEN_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED | TOKEN_ASSIGN_PRIMARY |
            TOKEN_DUPLICATE | TOKEN_IMPERSONATE | TOKEN_QUERY | TOKEN_QUERY_SOURCE |
            TOKEN_ADJUST_PRIVILEGES | TOKEN_ADJUST_GROUPS | TOKEN_ADJUST_DEFAULT |
            TOKEN_ADJUST_SESSIONID)
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        public enum TokenAccess
        {
            TOKEN_ASSIGN_PRIMARY = 0x0001,
            TOKEN_DUPLICATE = 0x0002,
            TOKEN_IMPERSONATE = 0x0004,
            TOKEN_QUERY = 0x0008,
            TOKEN_QUERY_SOURCE = 0x0010,
            TOKEN_ADJUST_PRIVILEGES = 0x0020,
            TOKEN_ADJUST_GROUPS = 0x0040,
            TOKEN_ADJUST_DEFAULT = 0x0080,
            TOKEN_ADJUST_SESSIONID = 0x0100,
            TOKEN_ALL_ACCESS_P = 0x000F00FF,
            TOKEN_ALL_ACCESS = 0x000F01FF,
            TOKEN_READ = 0x00020008,
            TOKEN_WRITE = 0x000200E0,
            TOKEN_EXECUTE = 0x00020000
        }

        public enum SecurityImpersonationLevel
        {
            SecurityAnonymous,
            SecurityIdentification,
            SecurityImpersonation,
            SecurityDelegation
        }

        public enum TokenType
        {
            TokenPrimary = 1,
            TokenImpersonation
        }
    }
}