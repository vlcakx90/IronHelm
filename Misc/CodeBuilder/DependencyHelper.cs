using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeBuilder
{
    public static class DependencyHelper
    {
        // Return Metadata List Refs for each assembly (.dll)
        public static List<MetadataReference> GetMetadataReferences(string assemblyPath)
        {
            string depExt = "*.dll";
            string[] depFiles = GetDependencyFiles(assemblyPath, depExt);
            List<MetadataReference> metaRefs = new List<MetadataReference>();    
            foreach (string depFile in depFiles)
            {
                metaRefs.Add(GetMetadataReference(depFile));
            }

            return metaRefs;
        }

        // Return Metadata Ref for assembly (.dll)
        public static MetadataReference GetMetadataReference(string assemblyPath)
        {
            return MetadataReference.CreateFromFile(assemblyPath);
        }

        // Get Files for each assembly (.dll)
        private static string[] GetDependencyFiles(string dependenciesPath, string depExt)
        {
            return Directory.GetFiles(dependenciesPath, depExt);
        }


        //-------------------------------------------------------------------- 
        //static string runtimePath = "C:\\Users\\vlcak\\source\\repos\\SharpTools\\Libraries\\CodeBuilder\\Libs\\{0}.dll";
        private static string runtimePath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\{0}.dll";        
        //private static string runtimePath2 = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.8\{0}.dll";       

        public static readonly IEnumerable<MetadataReference> CampCoreLibs =
            new[]
            {                
                //MetadataReference.CreateFromFile("C:\\Users\\vlcak\\source\\repos\\IronHelmBeta\\Camp\\Knight\\bin\\x64\\Release\\SharedArsenal.dll"),

                MetadataReference.CreateFromFile(string.Format(runtimePath, "Microsoft.CSharp")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Data.DataSetExtensions")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Data")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Net.Http")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Runtime.Serialization")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Xml")),
                MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Xml.Linq")),
            };

        public static string CampSharedArsenalLibPath { get; private set; } = string.Empty;
        public static MetadataReference? CampSharedArsenalLib { get; private set; } = null;

        public static void SetCampSharedArsenalLib(string  libPath)
        {
            WriteDebug.Info($"Setting SharedArsenal path: {libPath}");
            DependencyHelper.CampSharedArsenalLib = MetadataReference.CreateFromFile(libPath);
            CampSharedArsenalLibPath = libPath;
        }

        //public static readonly IEnumerable<MetadataReference> CampSharedArsenalLib =
        //    new[]
        //    {
        //        MetadataReference.CreateFromFile("C:\\Users\\vlcak\\source\\repos\\IronHelmBeta\\Camp\\Knight\\bin\\x64\\Release\\SharedArsenal.dll"),
        //    };

        //public static readonly IEnumerable<MetadataReference> DefaultReferences =                  
        //    new[]
        //    {
        //        MetadataReference.CreateFromFile(string.Format(runtimePath, "mscorlib")),
        //        MetadataReference.CreateFromFile(string.Format(runtimePath, "System")),
        //        MetadataReference.CreateFromFile(string.Format(runtimePath, "System.Core")),
        //        //MetadataReference.CreateFromFile(string.Format(runtimePath2, "System.Diagnostics")),
        //        //MetadataReference.CreateFromFile(string.Format(runtimePath2, "System.Windows.Forms")) // C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8\System.Windows.Forms.dll
        //    };

        //private static readonly IEnumerable<string> DefaultNamespaces =
        //    new[]
        //    {
        //        "System",
        //        //"system.diagnostics",
        //        //"System.Windows.Forms",
        //        //"System.IO",  //                                                                  
        //        //"System.Net",
        //        //"System.Linq",
        //        //"System.Text",
        //        //"System.Text.RegularExpressions",
        //        //"System.Collections.Generic"
        //    };

    }
}
