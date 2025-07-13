using Knight.Helpers;

using System;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;

namespace Knight.Models.Comm
{
    public class SmbCommModule : AllyCommModule
    {
        private readonly string _hostname;
        private readonly string _pipename;

        private NamedPipeServerStream _pipeServer;
        private NamedPipeClientStream _pipeClient;

        private CancellationTokenSource _cancellationTokenSource;
        public override bool Alive { get; protected set; }

        public override ConnectionMode ConnectionMode { get; }

        public override event Func<Raven.Raven, Task> RavenRecieved;
        public override event Action OnException;

        public SmbCommModule(string pipename) // SEVRER
        {
            ConnectionMode = ConnectionMode.SERVER;
            _pipename = pipename;
        }

        public SmbCommModule(string hostname, string pipename) // CLIENT
        {
            ConnectionMode = ConnectionMode.CLIENT;
            _hostname = hostname;
            _pipename = pipename;
        }

        public override void Init()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            switch (ConnectionMode)
            {
                case ConnectionMode.SERVER:
                    var pipeSec = new PipeSecurity();
                    var identity = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                    var pipeAccessRule = new PipeAccessRule(identity, PipeAccessRights.FullControl, System.Security.AccessControl.AccessControlType.Allow);
                    pipeSec.AddAccessRule(pipeAccessRule);

                    var serverStream = new NamedPipeServerStream(_pipename,PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous, 1024, 1024, pipeSec);

                    _pipeServer = serverStream;
                    break;

                case ConnectionMode.CLIENT:
                    var clientStream = new NamedPipeClientStream(_hostname, _pipename, PipeDirection.InOut, PipeOptions.Asynchronous);
                    
                    _pipeClient = clientStream;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override async Task Start()
        {
            switch (ConnectionMode)
            {
                case ConnectionMode.SERVER:
                    await _pipeServer.WaitForConnectionAsync();
                    break;

                case ConnectionMode.CLIENT:
                    var timeout = new CancellationTokenSource(new TimeSpan(0, 0, 30)); // 30 sec timeout
                    await _pipeClient.ConnectAsync(timeout.Token);

                    _pipeClient.ReadMode = PipeTransmissionMode.Byte;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Alive = true;
        }

        public override async Task Run()
        {
            PipeStream pipeStream = null;
            switch (ConnectionMode) // set stream source
            {
                case ConnectionMode.SERVER:
                    pipeStream = _pipeServer;
                    break;

                case ConnectionMode.CLIENT:
                    pipeStream = _pipeClient;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    if (pipeStream.DataAvailable())
                    {
                        var data = await pipeStream.ReadStream();
                        var raven = data.Deserialise<Raven.Raven>();

                        RavenRecieved?.Invoke(raven);
                    }
                }
                catch
                {
                    OnException?.Invoke();
                    return;
                }

                await Task.Delay(100);
            }

            _pipeServer?.Dispose();
            _pipeClient?.Dispose();
            _cancellationTokenSource?.Dispose();
        }

        public override void Stop()
        {
            _cancellationTokenSource.Cancel();
            Alive = false;
        }
        public override async Task SendRaven(Raven.Raven raven)
        {
            PipeStream pipeStream = null;
            switch (ConnectionMode) // set stream source
            {
                case ConnectionMode.SERVER:
                    pipeStream = _pipeServer;
                    break;

                case ConnectionMode.CLIENT:
                    pipeStream = _pipeClient;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            try
            {
                var data = raven.Serialise();
                await pipeStream.WriteStream(data);
            }
            catch
            {
                OnException?.Invoke();
            }
        }
    }
}
