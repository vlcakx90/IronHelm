using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Emit;
using System.Globalization;
using System.Reflection;
using static CodeBuilder.Compiler;

namespace CodeBuilder
{
    public static class Compiler
    {
        //private static string runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\";        // CHECK ME !!!!!!!!!!!!!!!!!!!!

        //public static async Task<byte[]> CompileSharedArsenalLib(string asmName, string sourcePath, CompileType compileType)
        //{
        //    // Macros -> will come from C2 Profile
        //    List<string> macros = new List<string>();
        //    macros.Add("XXX");

        //    List<MetadataReference> refs = DependencyHelper.CampCoreLibs.ToList();
        //    byte[] bytes = await Compile(asmName, sourcePath, compileType, refs, macros);

        //    return bytes;
        //}

        public static async Task<string?> BuildSharedArsenal(SharedArsenalOptions opt)
        {
            byte[]? bytes = await CompileSharedArsenal(opt);

            if (bytes == null) return null;            

            string path = opt.GetOutputFullPath();
            await System.IO.File.WriteAllBytesAsync(path, bytes);
            WriteDebug.Good($"Wrote {bytes.Length} bytes to {path}");

            return path;
        }

        //public static async Task<byte[]> CompileMB(string asmName, string sourcePath, CompileType compileType) 
        //{
        //    // Macros -> will come from C2 Profile
        //    List<string> macros = new List<string>();
        //    macros.Add("EXE");
        //    macros.Add("Get_ReadFromDisk");
        //    macros.Add("Format_None");
        //    macros.Add("Execute_Reflection");

        //    List<MetadataReference> refs = DependencyHelper.CampCoreLibs.ToList();
        //    refs.Add(DependencyHelper.CampSharedArsenalLib);

        //    byte[] bytes = await CompileEmbeddedResources(asmName, sourcePath, compileType, refs, macros);

        //    return bytes;
        //}

        public static async Task<string?> BuildMB(MBOptions opt)
        {
            byte[]? bytes = null;
            string? path = null;
            string? settingsFileText = null;

            // Files to be ignored and replaced
            List<string> ignoreFiles = new List<string>();
            ignoreFiles.Add("Settings.cs");
            List<string> addText = new List<string>();

            switch (opt.mBType)
            {
                case MB_Type.STAGED:

                    // Build Knight
                    WriteDebug.Info("Skipping Building Knight");

                    // Host Knight
                    WriteDebug.Info("Skipping Hosting Knight");

                    // Handle Settings
                    settingsFileText = await HandleSettings.MB(opt);
                    if (settingsFileText == null)
                    {
                        WriteDebug.Error("Failed to Handle Settings File");
                        return null;
                    }
                    WriteDebug.Info($"Adding New Settings File");
                    //Console.WriteLine($"    {settingsFileText}");
                    addText.Add(settingsFileText);

                    bytes = await CompileMB(opt, ignoreFiles, addText);
                    if (bytes == null) return null;

                    path = opt.GetOutputFullPath();
                    await System.IO.File.WriteAllBytesAsync(path, bytes);
                    WriteDebug.Good($"Wrote {bytes.Length} bytes to {path}");

                    return path;
                case MB_Type.STAGELESS:

                    // Build Knight
                    WriteDebug.Info("Skipping Building Knight");

                    // Add Knight as per specified method (Resource, Blob, but not as file)
                    WriteDebug.Info("Skipping Embedding Knight");

                    // Handle Settings
                    settingsFileText = await HandleSettings.MB(opt);
                    if (settingsFileText == null)
                    {
                        WriteDebug.Error("Failed to Handle Settings File");
                        return null;
                    }
                    WriteDebug.Info($"Adding New Settings File");
                    //Console.WriteLine($"    {settingsFileText}");
                    addText.Add(settingsFileText);

                    bytes = await CompileMB(opt, ignoreFiles, addText);
                    if (bytes == null) return null;

                    path = opt.GetOutputFullPath();
                    await System.IO.File.WriteAllBytesAsync(path, bytes);
                    WriteDebug.Good($"Wrote {bytes.Length} bytes to {path}");

                    return path;
                default:
                    return null;
            }            
        }

        //public static async Task<byte[]> CompileKnightTest2(string asmName, string sourcePath, CompileType compileType, KnightType knightType)
        //{
        //    // Macros -> will come from C2 Profile
        //    List<string> macros = new List<string>();
        //    //macros.Add("EXE");
        //    //macros.Add("Egress");
        //    macros.Add(compileType.ToString());
        //    macros.Add(knightType.ToString());

        //    // Debug
        //    Console.WriteLine($"[*] Macros: {macros.Count}");
        //    foreach (var m in macros)
        //    {
        //        Console.WriteLine($"\t{m}");
        //    }

        //    List<MetadataReference> refs = DependencyHelper.CampCoreLibs.ToList();
        //    refs.Add(DependencyHelper.CampSharedArsenalLib);

        //    byte[] bytes = await CompileEmbeddedResources(asmName, sourcePath, compileType, refs, macros);

        //    return bytes;
        //}

        public static async Task<string?> BuildKnight(KnightOptions opt)
        {            
            Console.WriteLine($"[*] Macros: {opt.macros.Count}");
            foreach (var m in opt.macros)
            {
                Console.WriteLine($"    {m}");
            }

            // Files to be ignored and replaced
            List<string> ignoreFiles = new List<string>();
            ignoreFiles.Add("Settings.cs");
            List<string> addText = new List<string>();

            // Handle Settings
            string? settingsFileText = await HandleSettings.Knight(opt);
            if (settingsFileText == null)
            {
                WriteDebug.Error("Failed to Handle Settings File");
                return null;
            }
            WriteDebug.Info($"Adding New Settings File");
            //Console.WriteLine($"    {settingsFileText}");
            addText.Add(settingsFileText);

            byte[]? bytes = await CompileKnight(opt, ignoreFiles, addText);
            if (bytes == null) return null;

            string path = opt.GetOutputFullPath();
            await System.IO.File.WriteAllBytesAsync(path, bytes);
            WriteDebug.Good($"Wrote {bytes.Length} bytes to {path}");

            return path;
        }

        private static async Task<byte[]?> CompileMB(MBOptions opt, List<string> ignoreFiles, List<string> addText)
        {
            //string asmName = "SharedArsenal";

            Console.WriteLine($"[*] Macros: {opt.macros.Count}");
            foreach (var m in opt.macros)
            {
                Console.WriteLine($"    {m}");
            }
            
            // Syntax Trees
            List<SyntaxTree> trees = await SyntaxTreeHelper.GetProjectSyntxTrees(opt.projectSourcePath, opt.macros, ignoreFiles, addText);
            Console.WriteLine($"[*] Syntax Trees Count: {trees.Count}");

            // Metadata of Dependencies (DLLs), so HardHatC2 has a copy of each Dll it needs, that's fine,
            // what if my program does not need every dll though?
            //List<MetadataReference> refs = DependencyHelper.GetMetadataReferences(runtimePath);
            //Console.WriteLine($"[*] MetadataRefs Count: {refs.Count}");

            //List<MetadataReference> refs = DependencyHelper.CampCoreLibs.ToList(); // This uses a hardcoded list of dependency .dlls
            Console.WriteLine($"[*] MetadataRefs Count: {opt.metaRefs.Count}");

            // Compiler Settings
            OptimizationLevel optimization = OptimizationLevel.Release; // Set optimization 
            Platform platform = Platform.X64;                           // Platform
            bool allowUnsafe = true;                                    // Unsafe code
            CSharpCompilationOptions options = new CSharpCompilationOptions(
                    outputKind: opt.GetOutputKind(),
                    optimizationLevel: optimization,
                    platform: platform,
                    allowUnsafe: allowUnsafe
                    );
            Console.WriteLine("[*] CSharpCompilationOptions");
            Console.WriteLine($"    OutputKind        : {options.OutputKind}");
            Console.WriteLine($"    optimizationLevel : {options.OptimizationLevel}");
            Console.WriteLine($"    platform          : {options.Platform}");
            Console.WriteLine($"    allowUnsafe       : {options.AllowUnsafe}");

            // Compile
            CSharpCompilation compilation = CSharpCompilation.Create(
                    assemblyName: opt.assemblyNameBase,
                    syntaxTrees: trees,
                    references: opt.metaRefs,
                    options: options
                    );

            //// Resource Test
            //string sharedArsenal = "SharedArsenal.dll";
            //string sharedArsenalPath = "C:\\Users\\vlcak\\source\\repos\\SharpTools\\Libraries\\CodeBuilder\\Libs\\" + sharedArsenal;

            //var resourceDescription = new Microsoft.CodeAnalysis.ResourceDescription(sharedArsenal, () => new FileStream(sharedArsenalPath, FileMode.Open), isPublic: true);
            //var resourceDescriptions = new List<ResourceDescription>() { resourceDescription };

            //// Check Result                         // NEED TO CHECK THIS CODE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!            
            return HandleCompilerOutput(ref compilation, opt);
        }
        private static async Task<byte[]?> CompileKnight(KnightOptions opt, List<string> ignoreFiles, List<string> addText)
        {
            //string asmName = "SharedArsenal";

            Console.WriteLine($"[*] Macros: {opt.macros.Count}");
            foreach (var m in opt.macros)
            {
                Console.WriteLine($"\t{m}");
            }

            // Syntax Trees
            List<SyntaxTree> trees = await SyntaxTreeHelper.GetProjectSyntxTrees(opt.projectSourcePath, opt.macros, ignoreFiles, addText);
            Console.WriteLine($"[*] Syntax Trees Count: {trees.Count}");

            // Metadata of Dependencies (DLLs), so HardHatC2 has a copy of each Dll it needs, that's fine,
            // what if my program does not need every dll though?
            //List<MetadataReference> refs = DependencyHelper.GetMetadataReferences(runtimePath);
            //Console.WriteLine($"[*] MetadataRefs Count: {refs.Count}");

            //List<MetadataReference> refs = DependencyHelper.CampCoreLibs.ToList(); // This uses a hardcoded list of dependency .dlls
            Console.WriteLine($"[*] MetadataRefs Count: {opt.metaRefs.Count}");

            // Compiler Settings
            OptimizationLevel optimization = OptimizationLevel.Release; // Set optimization 
            Platform platform = Platform.X64;                           // Platform
            bool allowUnsafe = true;                                    // Unsafe code
            CSharpCompilationOptions options = new CSharpCompilationOptions(
                    outputKind: opt.GetOutputKind(),
                    optimizationLevel: optimization,
                    platform: platform,
                    allowUnsafe: allowUnsafe
                    );
            Console.WriteLine("[*] CSharpCompilationOptions");
            Console.WriteLine($"    OutputKind        : {options.OutputKind}");
            Console.WriteLine($"    optimizationLevel : {options.OptimizationLevel}");
            Console.WriteLine($"    platform          : {options.Platform}");
            Console.WriteLine($"    allowUnsafe       : {options.AllowUnsafe}");

            // Compile
            CSharpCompilation compilation = CSharpCompilation.Create(
                    assemblyName: opt.assemblyNameBase,
                    syntaxTrees: trees,
                    references: opt.metaRefs,
                    options: options
                    );

            //// Resource Test
            //string sharedArsenal = "SharedArsenal.dll";
            //string sharedArsenalPath = "C:\\Users\\vlcak\\source\\repos\\SharpTools\\Libraries\\CodeBuilder\\Libs\\" + sharedArsenal;

            //var resourceDescription = new Microsoft.CodeAnalysis.ResourceDescription(sharedArsenal, () => new FileStream(sharedArsenalPath, FileMode.Open), isPublic: true);
            //var resourceDescriptions = new List<ResourceDescription>() { resourceDescription };

            //// Check Result                         // NEED TO CHECK THIS CODE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!            
            return HandleCompilerOutput(ref compilation, opt);
        }

        //private static async Task<byte[]> Compile(string asmName, string sourcePath, CompileType compileType, List<MetadataReference> refs, List<string> macros)
        private static async Task<byte[]?> CompileSharedArsenal(SharedArsenalOptions opt)
        {
            // Test
            List<string> ignoreFiles = new List<string>();
            List<string> addText = new List<string>();

            // Syntax Trees
            List<SyntaxTree> trees = await SyntaxTreeHelper.GetProjectSyntxTrees(opt.projectSourcePath, opt.macros, ignoreFiles, addText);
            Console.WriteLine($"[*] Syntax Trees Count: {trees.Count}");

            // Refs
            //Console.WriteLine($"[*] MetadataRefs Count: {refs.Count}");
            Console.WriteLine($"[*] MetadataRefs Count: {opt.metaRefs.Count}");

            // Set output type (exe, dll, service)
            OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary;            

            // Compiler Settings
            OptimizationLevel optimization = OptimizationLevel.Release; // Set optimization 
            Platform platform = Platform.X64;                           // Platform
            bool allowUnsafe = true;                                    // Unsafe code
            CSharpCompilationOptions options = new CSharpCompilationOptions(
                    //outputKind: outputKind,
                    outputKind: opt.GetOutputKind(),
                    optimizationLevel: optimization,
                    platform: platform,
                    allowUnsafe: allowUnsafe
                    );
            Console.WriteLine("[*] CSharpCompilationOptions");
            Console.WriteLine($"    OutputKind        : {options.OutputKind}");
            Console.WriteLine($"    optimizationLevel : {options.OptimizationLevel}");
            Console.WriteLine($"    platform          : {options.Platform}");
            Console.WriteLine($"    allowUnsafe       : {options.AllowUnsafe}");

            // Compile
            CSharpCompilation compilation = CSharpCompilation.Create(
                    assemblyName: opt.assemblyNameBase,
                    syntaxTrees: trees,
                    references: opt.metaRefs,
                    options: options
                    );

            //// Check Result                         // NEED TO CHECK THIS CODE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!            
            return HandleCompilerOutput(ref compilation, opt);
        }

        private static byte[]? HandleCompilerOutput(ref CSharpCompilation compilation, CompileOptions opt)
        {
            // Check Result                         // NEED TO CHECK THIS CODE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(
                    peStream: ms,
                    manifestResources: opt.dependencyResource
                    );

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);
                    WriteDebug.Error("Roslyn Compilation failed");

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("    >> {0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                        // Removed Broken Stuff
                    }
                    return null;
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    byte[] assemblyBytes = ms.ToArray();
                    return assemblyBytes;
                }
            }
        }
        private static byte[]? HandleCompilerOutputHardHatC2Version(ref CSharpCompilation compilation, CompileOptions opt)
        {
            // Check Result                         // NEED TO CHECK THIS CODE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(
                    peStream: ms,
                    manifestResources: opt.dependencyResource
                    );

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);
                    Console.WriteLine("[!] Roslyn Compilation failed");
                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("    >> {0}: {1}", diagnostic.Id, diagnostic.GetMessage());

                        // Print the code that caused the error
                        var lineSpan = diagnostic.Location.GetLineSpan();
                        var startLine = lineSpan.StartLinePosition.Line;
                        var endLine = lineSpan.EndLinePosition.Line;
                        if (lineSpan.Path != null && opt.projectSourcePath != null)
                        {
                            var sourceText = lineSpan.Path == "" ? opt.projectSourcePath : File.ReadAllText(lineSpan.Path); ////
                            var sourceLines = sourceText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                            for (int i = startLine; i <= endLine; i++)
                            {
                                Console.Error.WriteLine("Line {0}: {1}", i + 1, sourceLines[i]);
                            }

                            //if (sourceLines != null)
                            //{
                            //    int i = 1;
                            //    foreach (var line in sourceLines)
                            //    {
                            //        Console.Error.WriteLine("Line {0}: {1}", i++, line);
                            //    }
                            //}
                        }
                    }
                    return null;
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    byte[] assemblyBytes = ms.ToArray();
                    return assemblyBytes;
                }
            }
        }
    }
}
