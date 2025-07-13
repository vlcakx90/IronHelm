
using Knight.Helpers;
using Knight.Models.Knight;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Knight.Models.Comm
{
    public class TcpCommModule : AllyCommModule
    {
        private readonly string _remoteAddress;
        private readonly int _remotePort;

        private readonly IPAddress _localAddress;
        private readonly int _bindPort;

        private TcpListener _listener;
        private TcpClient _client;

        private CancellationTokenSource _cancellationTokenSource;

        public override event Func<Raven.Raven, Task> RavenRecieved;
        public override event Action OnException;

        public override bool Alive { get; protected set; }

        public override ConnectionMode ConnectionMode { get; }

        public TcpCommModule(bool loopBack, int bindPort = 4444) // SERVER >> Listen-for
        {
            ConnectionMode = ConnectionMode.SERVER;
            _localAddress = loopBack ? IPAddress.Loopback : IPAddress.Any;
            _bindPort = bindPort;               
        }

        public TcpCommModule(string remoteAddress, int remotePort = 4444) // CLIENT >> Connect-to
        {
            ConnectionMode = ConnectionMode.CLIENT;
            _remoteAddress = remoteAddress;
            _remotePort = remotePort;
        }

        public override void Init()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            switch (ConnectionMode)
            {
                case ConnectionMode.SERVER:
                    _listener = new TcpListener(new IPEndPoint(_localAddress, _bindPort));
                    _listener.Start(100);
                    break;

                case ConnectionMode.CLIENT:
                    _client = new TcpClient();
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
                    // await connection
                    _client = await _listener.AcceptTcpClientAsync();
                    // no longer listening
                    _listener.Stop();
                    break;
                    
                case ConnectionMode.CLIENT:
                    // connect to a listener
                    await _client.ConnectAsync(_remoteAddress, _remotePort);
                    break;

                default: throw new ArgumentOutOfRangeException();
            }

            Alive = true;
        }
        public override async Task Run()
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {

                try
                {
                    if (_client.DataAvailable())
                    {
                        var stream = _client.GetStream();
                        var data = await stream.ReadStream();
                        var raven = data.Deserialise<Raven.Raven>();

                        RavenRecieved?.Invoke(raven); // This is linked in Soldier.cs to HandleRaven
                    }
                }
                catch
                {
                    OnException?.Invoke();
                    return;
                }


                await Task.Delay(1000);
            }

            _client?.Dispose();
            _cancellationTokenSource.Dispose();

           //throw new System.NotImplementedException();
        }
        public override void Stop()
        {
            _cancellationTokenSource?.Cancel();
            Alive = false;
        }

        public override async Task SendRaven(Raven.Raven raven)
        {
            try
            {
                var data = raven.Serialise();
                var stream = _client.GetStream();

                await stream.WriteStream(data);
            }
            catch
            {
                OnException?.Invoke();
            }
        }
    }
}
