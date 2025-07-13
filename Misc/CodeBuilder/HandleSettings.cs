
using static CodeBuilder.Compiler;
using System.Globalization;
using System.Text;

namespace CodeBuilder
{
    public static class HandleSettings
    {
        private static C2Profile? _c2Profile; // For testing
        public static async Task<bool> InitC2Profile(string filePath) // Will be replaced by Castles C2ProfileService.Get...
        {
            var c2 = new C2ProfileService();
            _c2Profile = await c2.SetProfile(filePath);
            if (_c2Profile == null)
            {
                Console.WriteLine("[!] Failed to set C2Profile");
                return false;
            }
            c2.PrintCurrentProfile();

            return true;
        }
        public static async Task<string?> MB(MBOptions opt)
        {
            if (_c2Profile == null)
            {
                Console.WriteLine("[!] C2Profile Not Set");
                return null;
            }

            string? fileText = null;

            switch (opt.mBType)
            {
                case MB_Type.STAGED:
                    fileText = await MB_Staged(opt);
                    break;
                case MB_Type.STAGELESS:
                    fileText = await MB_Stageless(opt);
                    break;
                default:
                    fileText = null;
                    break;
            }

            return fileText;
        }
        private static async Task<string?> MB_Staged(MBOptions opt)
        {
            string? fileText = null;

            fileText = await GetSettingsFile(opt.projectSourcePath);
            if (fileText == null) return null;

            // Pass C2Profile.MB_Staged_Profile            
            fileText = fileText.Replace("{{R_SharedArsenal}}", $"{_c2Profile.SharedArsenal.ResourceName}");
            
            switch (_c2Profile.MB_Staged.Get)
            {
                case MB_Get_Staged.HttpUri:                    
                    fileText = fileText.Replace("{{R_HttpUri}}", $"{_c2Profile.MB_Staged.GetUri}");
                    break;
                case MB_Get_Staged.QUICUri:
                    break;
                case MB_Get_Staged.WebSocketUri:
                    break;
            }
            
            Console.WriteLine("\n================== Settings.cs");
            Console.WriteLine(fileText);
            Console.WriteLine("==================\n");

            return fileText;
        }

        private static async Task<string?> MB_Stageless(MBOptions opt)
        {
            string? fileText = null;

            fileText = await GetSettingsFile(opt.projectSourcePath);
            if (fileText == null) return null;

            // Pass C2Profile.MB_Staged_Profile here 
            fileText = fileText.Replace("{{R_SharedArsenal}}", $"{_c2Profile.SharedArsenal.ResourceName}");

            switch (_c2Profile.MB_Stageless.Get)
            {
                case MB_Get_Stageless.Resource:
                    fileText = fileText.Replace("{{R_Resource}", $"{_c2Profile.MB_Stageless.GetName}");
                    break;
                case MB_Get_Stageless.Blob:
                    fileText = fileText.Replace("{{R_Blob}}", $"{_c2Profile.MB_Stageless.GetName}");
                    break;
                case MB_Get_Stageless.File:
                    fileText = fileText.Replace("{{R_File}}", $"{_c2Profile.MB_Stageless.GetName}");
                    break;
            }

            Console.WriteLine("\n================== Settings.cs");
            Console.WriteLine(fileText);
            Console.WriteLine("==================\n");

            return fileText;
        }

        public static async Task<string?> Knight(KnightOptions opt)
        {
            if (_c2Profile == null)
            {
                Console.WriteLine("[!] C2Profile Not Set");
                return null;
            }

            string? fileText = null;

            switch (opt.KnightType)
            {
                case Knight_Type.Egress:
                    fileText = await Knight_Egress(opt);
                    break;
                case Knight_Type.P2P_NAMEDPIPE_SERVER:
                    fileText = await Knight_SMB_Server(opt);
                    break;
                case Knight_Type.P2P_NAMEDPIPE_CLIENT:
                    //fileText = await Knight_SMB_Client(opt);
                    break;
                case Knight_Type.P2P_TCP_SERVER:
                    fileText = await Knight_TCP_Server(opt);
                    break;
                case Knight_Type.P2P_TCP_CLIENT:
                    //fileText = await Knight_TCP_Client(opt);
                    break;
                default:
                    fileText = null;
                    break;
            }

            return fileText;
        }

        private static async Task<string?> Knight_Egress(KnightOptions opt)
        {
            string? fileText = null;

            fileText = await GetSettingsFile(opt.projectSourcePath);
            if (fileText == null) return null;

            WriteDebug.Info($"Skipping Settings Replace");

            // Ip
            fileText = fileText.Replace("{{R_CastleIp}}", $"{_c2Profile.Http.Ip}");
            // Port
            fileText = fileText.Replace("{{R_Port}}", $"{_c2Profile.Http.Port}");
            // TLS
            string tls = _c2Profile.Http.Tls == true ? "true" : "false";
            fileText = fileText.Replace("{{R_Tls}}", $"{tls}");
            // Header4Passwd
            fileText = fileText.Replace("{{R_HeaderForPasswd}}", $"{_c2Profile.Http.PasswdHeader}");
            // Header-Passwd
            fileText = fileText.Replace("{{R_Passwd}}", $"{_c2Profile.Http.Passwd}");
            // Get Paths
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i < _c2Profile.Http.GetPaths.Length; i++)
            {
                if (i == _c2Profile.Http.GetPaths.Length - 1)
                {
                    sb.Append($"\"{_c2Profile.Http.GetPaths[i]}\",");
                }
                else
                {
                    sb.Append($"\"{_c2Profile.Http.GetPaths[i]}\",");
                }
            }
            sb.Append("}");
            fileText = fileText.Replace("{{R_GetPaths}}", $"{sb.ToString()}");
            // Post Paths
            sb.Clear();
            sb.Append("{");
            for (int i = 0; i < _c2Profile.Http.PostPaths.Length; i++)
            {
                if (i == _c2Profile.Http.PostPaths.Length - 1)
                {
                    sb.Append($"\"{_c2Profile.Http.PostPaths[i]}\",");
                }
                else
                {
                    sb.Append($"\"{_c2Profile.Http.PostPaths[i]}\",");
                }
            }
            sb.Append("}");
            fileText = fileText.Replace("{{R_PostPaths}}", $"{sb.ToString()}");
            // Sleep
            fileText = fileText.Replace("{{R_Sleep}}", $"{_c2Profile.Http.Sleep}");
            // Jitter
            fileText = fileText.Replace("{{R_Jitter}}", $"{_c2Profile.Http.Jitter}");
            // SharedArsenal
            fileText = fileText.Replace("{{R_SharedArsenal}}", $"{_c2Profile.SharedArsenal.ResourceName}");

            Console.WriteLine("\n================== Settings.cs");
            Console.WriteLine(fileText);
            Console.WriteLine("==================\n");

            return fileText;
        }

        private static async Task<string?> Knight_SMB_Server(KnightOptions opt)
        {
            string? fileText = null;

            fileText = await GetSettingsFile(opt.projectSourcePath);
            if (fileText == null) return null;

            WriteDebug.Info($"Skipping Settings Replace");

            // Pipename
            fileText = fileText.Replace("{{R_Pipename}}", $"{_c2Profile.SMB.Pipename}");
            // SharedArsenal
            fileText = fileText.Replace("{{R_SharedArsenal}}", $"{_c2Profile.SharedArsenal.ResourceName}");

            Console.WriteLine("\n================== Settings.cs");
            Console.WriteLine(fileText);
            Console.WriteLine("==================\n");

            return fileText;
        }

        //private static async Task<string?> Knight_SMB_Client(KnightOptions opt)
        //{
        //    string? fileText = null;

        //    fileText = await GetSettingsFile(opt.projectSourcePath);
        //    if (fileText == null) return null;

        //    WriteDebug.Info($"Skipping Settings Replace");

        //    // Pipename
        //    fileText = fileText.Replace("{{R_Pipename}}", $"{_c2Profile.SMB.Pipename}");
        //    // Hostname
        //    fileText = fileText.Replace("{{R_Hostname}{", $"{_c2Profile.SMB.}"); // Needs to be passed as arg
        //    // SharedArsenal
        //    fileText = fileText.Replace("{{R_SharedArsenal}}", $"{_c2Profile.SharedArsenal.ResourceName}");

        //    return fileText;
        //}

        private static async Task<string?> Knight_TCP_Server(KnightOptions opt)
        {
            string? fileText = null;

            fileText = await GetSettingsFile(opt.projectSourcePath);
            if (fileText == null) return null;

            WriteDebug.Info($"Skipping Settings Replace");

            // Loopback
            string loopback = _c2Profile.TCP.Loopback == true ? "true" : "false";
            fileText = fileText.Replace("{{R_Loopback}}", $"{loopback}");
            // Port
            fileText = fileText.Replace("{{R_Port}}", $"{_c2Profile.TCP.Port}");
            // SharedArsenal
            fileText = fileText.Replace("{{R_SharedArsenal}}", $"{_c2Profile.SharedArsenal.ResourceName}");

            Console.WriteLine("\n================== Settings.cs");
            Console.WriteLine(fileText);
            Console.WriteLine("==================\n");

            return fileText;
        }

        //private static async Task<string?> Knight_TCP_Client(KnightOptions opt)
        //{
        //    string? fileText = null;

        //    fileText = await GetSettingsFile(opt.projectSourcePath);
        //    if (fileText == null) return null;

        //    WriteDebug.Info($"Skipping Settings Replace");

        //    // Remoteaddress
        //    fileText = fileText.Replace("{{R_RemoteAddress}", $"{_c2Profile.}"); // // Needs to be passed as arg
        //    // Port
        //    fileText = fileText.Replace("{{R_Port}", $"{_c2Profile.}"); // // Needs to be passed as arg
        //    // SharedArsenal
        //    fileText = fileText.Replace("{{R_SharedArsenal}}", $"{_c2Profile.SharedArsenal.ResourceName}");

        //    return fileText;
        //}

        private static async Task<string?> GetSettingsFile(string projectSourcePath)
        {
            string file = "Settings.cs";
            string path = Path.Combine(projectSourcePath, file);

            if (!System.IO.File.Exists(path))
            {
                return null;
            }
            Console.WriteLine($">> Settings path: {path}");
            return await File.ReadAllTextAsync(path);
        }
    }
}
