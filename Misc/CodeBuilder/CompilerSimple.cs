//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis.Text;
//using System.Text;

//namespace CodeBuilder
//{
//    public static class CompilerSimple // Source: (https://stackoverflow.com/questions/32769630/how-to-compile-a-c-sharp-file-with-roslyn-programmatically)
//    {
//        public static async Task<bool> Compile(string src)
//        {
//            //var fileToCompile = @"C:\Users\DesktopHome\Documents\Visual Studio 2013\Projects\ConsoleForEverything\SignalR_Everything\Program.cs";

//            if (!File.Exists(src))
//            {
//                Console.WriteLine($"[!] File {src} not found");
//                return false;
//            }

//            string outFile = src.Replace(".cs", ".dll");

//            var source = await File.ReadAllTextAsync(src);

//            var parsedSyntaxTree = Parse(source, "", CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp11));

//            CSharpCompilationOptions DefaultCompilationOptions = 
//                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary) // Format
//                    .WithOverflowChecks(true).WithOptimizationLevel(OptimizationLevel.Release) // Optimization
//                    .WithUsings(DefaultNamespaces); // Namespace

//            var compilation = CSharpCompilation.Create("out.dll", new SyntaxTree[] { parsedSyntaxTree }, DefaultReferences, DefaultCompilationOptions);
//            try
//            {
//                var result = compilation.Emit(outFile);

//                if (result.Success) 
//                {
//                    Console.WriteLine($"[+] Compiled to {outFile}");
//                    return true;

//                }
//                else
//                {
//                    Console.WriteLine($"[!] Failed to Compile");

//                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
//                        diagnostic.IsWarningAsError ||
//                        diagnostic.Severity == DiagnosticSeverity.Error);
//                    Console.WriteLine(" Roslyn Compilation failed");
//                    foreach (Diagnostic diagnostic in failures)
//                    {
//                        Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());

//                        // Print the code that caused the error
//                        var lineSpan = diagnostic.Location.GetLineSpan();
//                        var startLine = lineSpan.StartLinePosition.Line;
//                        var endLine = lineSpan.EndLinePosition.Line;
//                        var sourceText = lineSpan.Path == "" ? source : File.ReadAllText(lineSpan.Path);
//                        var sourceLines = sourceText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

//                        for (int i = startLine; i <= endLine; i++)
//                        {
//                            Console.Error.WriteLine("Line {0}: {1}", i + 1, sourceLines[i]);
//                        }
//                    }

//                    return false;
//                }

//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex);
//                return false;
//            }
//        }

//        // C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2
//        private static string runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\{0}.dll";        // CHECK ME !!!!!!!!!!!!!!!!!!!!
//        private static string runtimePath2 = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.8\{0}.dll";        // CHECK ME !!!!!!!!!!!!!!!!!!!!

//        private static readonly IEnumerable<string> DefaultNamespaces =
//            new[]
//            {
//                "System",
//                //"system.diagnostics",
//                //"System.Windows.Forms",
//                //"System.IO",  //                                                                  WHAT HAPPENS IF THESE ARE INCLUDED BUT NOT USED BY SOURCE -> they will not be included (are namespaces anyway)
//                //"System.Net",
//                //"System.Linq",
//                //"System.Text",
//                //"System.Text.RegularExpressions",
//                //"System.Collections.Generic"
//            };


//        private static readonly IEnumerable<MetadataReference> DefaultReferences =                  // CHECK ME !!!!!!!!!!!!!!!!!!!!!!!!1
//            new[]
//            {
//                MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
//                MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
//                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core")),
//                //MetadataReference.CreateFromFile(string.Format(runtimePath2, "System.Diagnostics")),
//                //MetadataReference.CreateFromFile(string.Format(runtimePath2, "System.Windows.Forms")) // C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Windows.Forms.dll
//            };

//        public static SyntaxTree Parse(string text, string filename = "", CSharpParseOptions options = null)
//        {
//            var stringText = SourceText.From(text, Encoding.UTF8);
//            return SyntaxFactory.ParseSyntaxTree(stringText, options, filename);
//        }
//    }
//}
