using Knight.Models.Comm;
using Knight.Models.Raven;
using System;


namespace Knight.Commands
{
    public class TcpServer : KnightCommand
    {
        public override string Name => "tcp_server";

        public override string Execute(TaskMessage task)
        {
            var loopback = task.Arguments[0] == "loopback" ? true : false;  // CHANGE THIS ???
            var port = Int32.Parse(task.Arguments[1]);

            AllyCommModule commModule = new TcpCommModule(loopback, port);

            Knight.AddChildAlly(task.TaskId, commModule);

            return $"Started Tcp Server for {loopback}(loopback):{port}";
        }
    }
}
