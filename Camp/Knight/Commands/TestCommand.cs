using Knight.Models.Raven;

namespace Knight.Commands
{
    public class TestCommand : KnightCommand
    {
        public override string Name => "TestCommand";

        public override string Execute(TaskMessage task)
        {
            return "Hello from TestCommand";
        }
    }
}