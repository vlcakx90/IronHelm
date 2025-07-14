using Microsoft.CodeAnalysis;

namespace CodeBuilder
{
    public static class Program
    {
        static string c2ProfilePath = "C:\\Users\\vlcak\\source\\repos\\IronHelm\\Misc\\CodeBuilder\\test.yaml";
        static string sharedarsenalProjectPath = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\SharedArsenal\\";
        static string mbProjectPath = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\MB\\";
        static string knightProjectPath = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\Knight\\";
        static string outBasePath = "C:\\Users\\vlcak\\Desktop\\Test\\";
        static string outSharedArsenal = "C:\\Users\\vlcak\\source\\repos\\IronHelm\\Misc\\CodeBuilder\\Libs\\";


        static string spacer = new string('-', 25);
        public static async Task Main(string[] args)
        {
            if (!await HandleSettings.InitC2Profile(c2ProfilePath)) // Set C2Profile
            {
                Console.WriteLine("[!] Failed to Set C2Profile");
                return;
            }
            await BuildCamp();
        }        
        private static async Task BuildCamp()
        {
            ////// SharedArsenal
            Console.WriteLine("\n" + spacer + $"Compiling: SharedArsenal");
            await BuildArsenal();

            ////// Knights
            Console.WriteLine("\n" + spacer + $"Compiling: Knight with {Knight_Type.Egress}");
            await BuildKnight(Knight_Type.Egress);
            Console.WriteLine("\n" + spacer + $"Compiling: Knight with {Knight_Type.P2P_TCP_SERVER}");
            await BuildKnight(Knight_Type.P2P_TCP_SERVER);            
            Console.WriteLine("\n" + spacer + $"Compiling: Knight with {Knight_Type.P2P_SMB_SERVER}");
            await BuildKnight(Knight_Type.P2P_SMB_SERVER);

            // Clients will be supported later
            //Console.WriteLine("\n" + spacer + $"Compiling: Knight with {Knight_Type.P2P_TCP_CLIENT}");
            //await BuildKnight(Knight_Type.P2P_TCP_CLIENT);
            //Console.WriteLine("\n" + spacer + $"Compiling: Knight with {Knight_Type.P2P_SMB_CLIENT}");
            //await BuildKnight(Knight_Type.P2P_SMB_CLIENT);

            ////// MB
            Console.WriteLine("\n" + spacer + $"Compiling: MB Staged");
            await BuildMBStaged();
            Console.WriteLine("\n" + spacer + $"Compiling: MB Stageless");
            await BuildMBStageless();
        }

        private static async Task BuildArsenal()
        {
            string? outPath = await Compiler.BuildSharedArsenal(new SharedArsenalOptions
            {
                assemblyNameBase = "SharedArsenal",
                outputDirectory = outSharedArsenal,
                //projectSourcePath = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\SharedArsenal\\",
                projectSourcePath = sharedarsenalProjectPath,
                metaRefs = DependencyHelper.CampCoreLibs.ToList(),
                macros = TempGetSharedArsenalMacros(),
                compileType = Compile_Type.DLL
            });

            if (outPath == null) return;

            DependencyHelper.SetCampSharedArsenalLib(outPath);
        }

        private static async Task BuildMBStaged()
        {
            string resourcePath = DependencyHelper.CampSharedArsenalLibPath;
            int index = resourcePath.LastIndexOf('\\');
            string resource = resourcePath.Substring(index + 1, resourcePath.Length - index - 1);
            var resourceDescription = new Microsoft.CodeAnalysis.ResourceDescription(resource, () => new FileStream(resourcePath, FileMode.Open), isPublic: true);
            var resourceDescriptions = new List<ResourceDescription>() { resourceDescription };

            List<MetadataReference> refs = DependencyHelper.CampCoreLibs.ToList();
            MetadataReference? metaRef = DependencyHelper.CampSharedArsenalLib;
            if (metaRef != null) refs.Add(metaRef);

            string? outPath = await Compiler.BuildMB(new MBOptionsStaged
            {
                assemblyNameBase = "MB",
                outputDirectory = outBasePath,
                //projectSourcePath = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\MB\\",
                projectSourcePath = mbProjectPath,
                metaRefs = refs,
                macros = TempGetMBStagedMacros(),
                dependencyResource = resourceDescriptions,
                compileType = Compile_Type.EXE
            });
        }

        private static async Task BuildMBStageless()
        {
            string resourcePath = DependencyHelper.CampSharedArsenalLibPath;
            int index = resourcePath.LastIndexOf('\\');
            string resource = resourcePath.Substring(index + 1, resourcePath.Length - index - 1);
            var resourceDescription = new Microsoft.CodeAnalysis.ResourceDescription(resource, () => new FileStream(resourcePath, FileMode.Open), isPublic: true);
            var resourceDescriptions = new List<ResourceDescription>() { resourceDescription };

            List<MetadataReference> refs = DependencyHelper.CampCoreLibs.ToList();
            MetadataReference? metaRef = DependencyHelper.CampSharedArsenalLib;
            if (metaRef != null) refs.Add(metaRef);

            string? outPath = await Compiler.BuildMB(new MBOptionsStageless
            {
                assemblyNameBase = "MB",
                outputDirectory = outBasePath,
                //projectSourcePath = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\MB\\",
                projectSourcePath = mbProjectPath,
                metaRefs = refs,
                macros = TempGetMBStagelessMacros(),
                dependencyResource = resourceDescriptions,
                compileType = Compile_Type.EXE
            });
        }

        private static async Task BuildKnight(Knight_Type knightType)
        {
            string resourcePath = DependencyHelper.CampSharedArsenalLibPath;
            int index = resourcePath.LastIndexOf('\\');
            string resource = resourcePath.Substring(index + 1, resourcePath.Length - index - 1);
            var resourceDescription = new Microsoft.CodeAnalysis.ResourceDescription(resource, () => new FileStream(resourcePath, FileMode.Open), isPublic: true);
            var resourceDescriptions = new List<ResourceDescription>() { resourceDescription };

            List<MetadataReference> refs = DependencyHelper.CampCoreLibs.ToList();
            MetadataReference? metaRef = DependencyHelper.CampSharedArsenalLib;
            if (metaRef != null) refs.Add(metaRef);

            string? outPath = await Compiler.BuildKnight(new KnightOptions
            {
                assemblyNameBase = "Knight_" + $"{knightType}",
                outputDirectory = outBasePath,
                //projectSourcePath = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\Knight\\",
                projectSourcePath = knightProjectPath,
                metaRefs = refs,
                macros = TempGetKnightMacros(Compile_Type.EXE, knightType),
                dependencyResource = resourceDescriptions,
                compileType = Compile_Type.EXE,
                KnightType = knightType
            });
        }
        //private static async Task CompKnight(string spacer, string sa, string outBasePath, Compiler.CompileType ct, Compiler.KnightType kt)
        //{
        //    //Compiler.KnightType kt = Compiler.KnightType.Egress;
        //    bool tls = false;
        //    Console.WriteLine(spacer + $"Compiling Knight: {kt}");
        //    string source = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\Knight\\";
        //    string asmName = $"Knight_{kt}.exe";
        //    string asmOut = outBasePath + asmName; // $"Knight_{kt}.exe"; // Hardcoded exe
        //    byte[] asmBytes = await Compiler.CompileKnightTest2(asmName, source, ct, kt);
        //    if (asmBytes == null) { Console.WriteLine("[!] Compiler Failed"); return; }
        //    Console.WriteLine($"[+] Wrote {asmBytes.Length} bytes to {asmOut}\n");
        //    File.WriteAllBytes(asmOut, asmBytes);            
        //}

        #region TempMacros
        private static List<string> TempGetSharedArsenalMacros()
        {
            List<string> macros = new List<string>();
            macros.Add("XXX");
            return macros;
        }

        private static List<string> TempGetMBStagedMacros()
        {
            List<string> macros = new List<string>();
            macros.Add("EXE");
            macros.Add("HttpUri");
            macros.Add("None");
            macros.Add("ExecuteAssembly");
            return macros;
        }

        private static List<string> TempGetMBStagelessMacros()
        {
            List<string> macros = new List<string>();
            macros.Add("EXE");
            macros.Add("File");
            macros.Add("None");
            macros.Add("ExecuteAssembly");
            //macros.Add("Execute_Shellcode_SpawnInject");
            return macros;
        }
        private static List<string> TempGetKnightMacros(Compile_Type compType, Knight_Type knightType)
        {
            List<string> macros = new List<string>();
            macros.Add(compType.ToString());
            macros.Add(knightType.ToString());
            return macros;
        }
        #endregion

        ////private static async Task CampTest()
        ////{
        ////    // Out Base
        ////    string outBasePath = "C:\\Users\\vlcak\\Desktop\\Test\\RosylnOut_";

        ////    // SharedArsenal
        ////    Console.WriteLine("Compiling SharedArsenal");
        ////    string source = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\SharedArsenal\\";
        ////    string asmOut = outBasePath + "SharedArsenal.dll";
        ////    byte[] asmBytes = await Compiler.CompileSharedArsenalLib(source, Compiler.CompileType.DLL);
        ////    if (asmBytes == null) { Console.WriteLine("[!] Compiler Failed"); return; }
        ////    Console.WriteLine($"[+] Wrote {asmBytes.Length} bytes to {asmOut}\n");
        ////    File.WriteAllBytes(asmOut, asmBytes);

        ////    // Set Path For SharedArsenal
        ////    Console.WriteLine("[*] Setting path for SharedArsenal lib");
        ////    Console.WriteLine($"    {asmOut}");
        ////    DependencyHelper.SetCampSharedArsenalLib(asmOut);

        ////    // MB Loader Things
        ////    Console.WriteLine("Compiling MB");
        ////    source = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\MB\\";
        ////    asmOut = outBasePath + "MB.exe";
        ////    asmBytes = await Compiler.CompileMB(source, Compiler.CompileType.EXE);
        ////    if (asmBytes == null) { Console.WriteLine("[!] Compiler Failed"); return; }
        ////    Console.WriteLine($"[+] Wrote {asmBytes.Length} bytes to {asmOut}\n");
        ////    File.WriteAllBytes(asmOut, asmBytes);

        ////    // Egress Knight
        ////    Console.WriteLine("Compiling Knight");
        ////    source = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\Knight\\";
        ////    asmOut = outBasePath + "Knight.exe";
        ////    asmBytes = await Compiler.CompileKnight(source, Compiler.CompileType.EXE);
        ////    if (asmBytes == null) { Console.WriteLine("[!] Compiler Failed"); return; }
        ////    Console.WriteLine($"[+] Wrote {asmBytes.Length} bytes to {asmOut}\n");
        ////    File.WriteAllBytes(asmOut, asmBytes);

        ////    // P2P Knight
        ////    Console.WriteLine("Compiling Knight");
        ////    source = "C:\\Users\\vlcak\\source\\repos\\ironHelm\\Camp\\Knight\\";
        ////    asmOut = outBasePath + "Knight_P2P.exe";
        ////    asmBytes = await Compiler.CompileKnightP2P(source, Compiler.CompileType.EXE);
        ////    if (asmBytes == null) { Console.WriteLine("[!] Compiler Failed"); return; }
        ////    Console.WriteLine($"[+] Wrote {asmBytes.Length} bytes to {asmOut}\n");
        ////    File.WriteAllBytes(asmOut, asmBytes);
        ////}

        //private static async Task MacrosTest()
        //{
        //    string outBasePath = "C:\\Users\\vlcak\\Desktop\\Test\\HelloWorld_";

        //    List<string> macros = new List<string>();

        //    //// Default
        //    //macros.Add("X ");
        //    //Console.WriteLine("Compiling Hello World");
        //    //string source = "C:\\Users\\vlcak\\source\\repos\\SharpTools\\HelloWorld\\";
        //    //string asmOut = outBasePath + "HelloWorld.exe";
        //    //byte[] asmBytes = await Compiler.CompileSharedArsenalLib(source, Compiler.CompileType.Exe, macros);
        //    //if (asmBytes == null) { Console.WriteLine("[!] Compiler Failed"); return; }
        //    //Console.WriteLine($"[+] Wrote {asmBytes.Length} bytes to {asmOut}\n");
        //    //File.WriteAllBytes(asmOut, asmBytes);
        //    //macros.Clear();

        //    // MOFO 
        //    string m = "MOFO";
        //    macros.Add(m);
        //    Console.WriteLine("Compiling Hello World");
        //    string source = "C:\\Users\\vlcak\\source\\repos\\SharpTools\\HelloWorld\\";
        //    string asmOut = outBasePath + $"{m}.exe";
        //    byte[] asmBytes = await Compiler.CompileSharedArsenalLib("MOFO", source, Compiler.CompileType.EXE);
        //    if (asmBytes == null) { Console.WriteLine("[!] Compiler Failed"); return; }
        //    Console.WriteLine($"[+] Wrote {asmBytes.Length} bytes to {asmOut}\n");
        //    File.WriteAllBytes(asmOut, asmBytes);
        //    macros.Clear();

        //    // BOO
        //    m = "BOO";
        //    macros.Add(m);
        //    Console.WriteLine("Compiling Hello World");
        //    source = "C:\\Users\\vlcak\\source\\repos\\SharpTools\\HelloWorld\\";
        //    asmOut = outBasePath + $"{m}.exe";
        //    asmBytes = await Compiler.CompileSharedArsenalLib("BOO", source, Compiler.CompileType.EXE);
        //    if (asmBytes == null) { Console.WriteLine("[!] Compiler Failed"); return; }
        //    Console.WriteLine($"[+] Wrote {asmBytes.Length} bytes to {asmOut}\n");
        //    File.WriteAllBytes(asmOut, asmBytes);
        //    macros.Clear();
        //}
        //private static async Task SimpleTest()
        //{
        //    Console.WriteLine("CompilerSimple Test");
        //    string source = "C:\\Users\\vlcak\\source\\repos\\SharpTools\\HelloWorld\\Program.cs";
        //    Console.WriteLine($"Compiling {source}");
        //    await CompilerSimple.Compile(source);

        //    Console.WriteLine("\n\n");

        //    source = "C:\\Users\\vlcak\\source\\repos\\SharpTools\\PopMessageBox\\Program.cs";
        //    Console.WriteLine($"Compiling {source}");
        //    await CompilerSimple.Compile(source);
        //}
    }
}
