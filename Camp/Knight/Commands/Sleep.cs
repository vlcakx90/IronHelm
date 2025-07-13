#if Egress
using Knight.Models.Raven;

namespace Knight.Commands
{
    public class Sleep : KnightCommand
    {
        public override string Name => "sleep";

        public override string Execute(TaskMessage task)
        {
            string result;

            if (task.Arguments == null || task.Arguments.Length < 2)
            {
                result = Settings.GetSleepJitter();
            }
            else
            {
                result = Settings.SetSleepJitter(task.Arguments[0], task.Arguments[1]);
            }

            return result;
        }
    }
}
#endif