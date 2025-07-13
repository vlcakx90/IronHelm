using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using System.Net;


namespace Castle.Models.Commander
{
    public class HttpCommander : Commander
    {
        public override COMMANDER_TYPE Type { get; }

        public override string Name { get; }

        public override int BindPort { get; }

        //public override string Passwd {  get; }

        public override bool Tls {  get; }

        public string FileServerDirectory { get; set; }

        private CancellationTokenSource _tokenSource;

        public HttpCommander(string name, int bindPort, bool tls)
        {
            Type = COMMANDER_TYPE.HTTP;
            Name = name;
            BindPort = bindPort;
            //Passwd = passwd;
            Tls = tls;
        }
        public override async Task Start()
        {
            CreateFileDirectory();

            var hostBuilder = new HostBuilder()
                .ConfigureWebHostDefaults(host =>
                {
                    //host.UseUrls($"https://0.0.0.0:{BindPort}");
                    host.UseUrls(Tls ? $"https://0.0.0.0:{BindPort}" : $"http://0.0.0.0:{BindPort}");
                    host.Configure(ConfigureApp);
                    host.ConfigureServices(ConfigureServices);
                    host.ConfigureKestrel(ConfigureKrestel);
                });

            var host = hostBuilder.Build();

            _tokenSource = new CancellationTokenSource();
            await host.RunAsync(_tokenSource.Token);
        }

        private void CreateFileDirectory()
        {
            FileServerDirectory = Path.Combine(Directory.GetCurrentDirectory(), "FileServer", Name);

            if (!Directory.Exists(FileServerDirectory))
            {
                Directory.CreateDirectory(FileServerDirectory);
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(SoldierService);
            services.AddSingleton(RavenService);
            services.AddSingleton(C2ProfileService);
        }

        private void ConfigureApp(IApplicationBuilder app)
        {
            //const string getPath = @"/index.php";
            //const string postPath = @"/submit.php";            

            //app.UseRouting();
            //// What URIs the Castle will respond on
            //app.UseEndpoints(e =>
            //{
            //    e.MapControllerRoute(getPath, getPath, new // Can get Paths from C2 profile ...
            //    {
            //        controller = "HttpCommander",
            //        action = "HandleImplant"
            //    });                
            //    e.MapControllerRoute(postPath, postPath, new // Can get Paths from C2 profile ...
            //    {
            //        controller = "HttpCommander",
            //        action = "HandleImplant"
            //    });
            //});

            ////////
            ///         Paths now with C2Profile
            ///////
            var paths = C2ProfileService.ProfileHttpAllPaths();

            app.UseRouting();
            // What URIs the Castle will respond on
            app.UseEndpoints(e =>
            {
                foreach (var path in paths)
                {
                    e.MapControllerRoute(path, path, new // Can get Paths from C2 profile ...
                    {
                        controller = "HttpCommander",
                        action = "HandleImplant",                        
                    });
                    //e.MapControllerRoute(path, path, null, null, null);
                }
            });

            /////////
            ///         CAN ADD STATIC FILE HOSTING HERE
            /////////
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(FileServerDirectory),
                ServeUnknownFileTypes = true,
                DefaultContentType = "text/plain",
            });
        }

        private void ConfigureKrestel(KestrelServerOptions krestel)
        {
            krestel.AddServerHeader = false;
            krestel.Listen(IPAddress.Any, BindPort, ListenOptions);
        }

        private void ListenOptions(ListenOptions options)
        {
            // Set Http Type
            options.Protocols = HttpProtocols.Http1AndHttp2;
            //options.Protocols = HttpProtocols.Http3; // QUIC

            // Set TLS + Self Signed Cert            
            if (Tls)
            {
                var cert = Helpers.Certificates.GetSelfSigned(Name);
                options.UseHttps(cert);
            }
        }

        public override void Stop()
        {
            _tokenSource.Cancel();
            _tokenSource.Dispose();

            Directory.Delete(FileServerDirectory, true );
        }
    }
}