
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace CodeBuilder
{
    public static class SyntaxTreeHelper
    {
        // get syntax trees for entire project
        public static async Task<List<SyntaxTree>> GetProjectSyntxTrees(string projectPath, List<string> macros, List<string> ignoreFiles, List<string> addText)
        {
            string[] ignoreDirs = { "bin", "obj", "Properties" };
            List<string> srcFiles = GetFilesByExtension(projectPath, ignoreDirs).ToList();
            List<string> srcTexts = await GetFileTexts(srcFiles, ignoreFiles);
            foreach (string text in addText) srcTexts.Add(text);

            return ToSyntaxTrees(srcTexts, macros);
        }

        // get syntax tree for single file
        public static async Task<SyntaxTree?> GetProjectSyntxTree(string srcPath, List<string> macros)
        {
            string? srcText = await GetFileText(srcPath);
            if (srcText == null)
            {
                return null;
            }

            return ToSyntaxTree(srcText, macros);
        }

        

        // list of fileText to SyntaxTrees
        private static List<SyntaxTree> ToSyntaxTrees(List<string> fileTexts, List<string> macros) // OG
        {
            List<SyntaxTree> trees = new List<SyntaxTree>();
            foreach (var fileText in fileTexts)
            {
                SyntaxTree? tree = ToSyntaxTree(fileText, macros);

                if (tree != null)
                {
                    trees.Add(tree);
                }
            }

            return trees;
        }

        // fileText to SyntaxTree
        private static SyntaxTree? ToSyntaxTree(string fileText, List<string> macros)
        {
            //////
            //List<string> macros = new List<string>();
            //macros.Add("MOFO");
            CSharpParseOptions opt = new CSharpParseOptions(preprocessorSymbols: macros);
            ////////

            SyntaxTree tree = CSharpSyntaxTree.ParseText(text: fileText, options: opt);
            //SyntaxTree tree = CSharpSyntaxTree.ParseText(fileText);

            if (tree.Length == 0)
            {
                return null;
            }

            return tree;
        }

        // list of paths to FileText
        private static async Task<List<string>> GetFileTexts(List<string> filePaths, List<string> ignoreFiles)
        {
            List<string> fileTexts = new List<string>();

            foreach (var file in filePaths)
            {
                // Check if in ignoreFiles
                string filename = GetFilenameFromPath(file);
                if (ignoreFiles.Contains(filename))
                {
                    WriteDebug.Info($"Ignoring File: {file} -> {filename}");
                    continue;
                }

                string? fileText = await GetFileText(file);
                if (fileText != null)
                {
                    fileTexts.Add(fileText);
                }
            }

            return fileTexts;
        }

        // FileText
        private static async Task<string?> GetFileText(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return null;
            }

            return await File.ReadAllTextAsync(filePath);
        }

        // Check if directory is ignored
        private static bool IsIgnoredDir(string subDir, string[] ignoreDirs)
        {
            foreach (var dir in ignoreDirs)
            {
                if (subDir.EndsWith(dir))
                {
                    return true;
                }
            }

            return false;
        }
        
        // Get files by ext recursive
        private static IEnumerable<string> GetFilesByExtension(string path, string[] ignoreDirs, string ext = "*.cs") // https://stackoverflow.com/questions/929276/how-to-recursively-list-all-the-files-in-a-directory-in-c
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(GetPath(path));

            Console.WriteLine($"[*] Getting Source Files");

            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        //Console.WriteLine($"SubDir: {subDir}");
                        if (!IsIgnoredDir(subDir, ignoreDirs))
                        {
                            queue.Enqueue(subDir);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                string[]? files = null;

                try
                {
                    files = Directory.GetFiles(path, ext);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }

                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        //Console.WriteLine($"    {files[i]}");
                        yield return files[i];
                    }
                }
            }
        }

        private static string GetPath(string fullPath)
        {
            if (!fullPath.EndsWith("\\"))
            {
                return Directory.GetParent(fullPath).FullName + "\\";
            }
            else
            {
                return fullPath;
            }
        }

        private static string GetFilenameFromPath(string path)
        {
            int index = path.LastIndexOf('\\');
            
            return path.Substring(index + 1, path.Length - index - 1);
        }
    }
}
