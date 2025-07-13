using Knight.Models.Raven;

namespace Knight.Commands
{
    public class Exit : KnightCommand
    {
        public override string Name => "exit";

        public override string Execute(TaskMessage task)
        {
            return "exiting";
        }
    }
}