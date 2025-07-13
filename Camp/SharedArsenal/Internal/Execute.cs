using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace SharedArsenal.Internal
{
    public static class Execute
    {
        public static string ExecuteCommand(string fileName, string arguments)
        {

            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var process = Process.Start(startInfo);
            string output = "";

            using (process.StandardOutput)
            {
                output += process.StandardOutput.ReadToEnd();
            }

            using (process.StandardError)
            {
                output += process.StandardError.ReadToEnd();
            }

            return output;
        }


        // Many assemblies (Like our TestApp1 or Rubeus) will write to console (like Console.WriteLine())
        // this is execeuted inside our agent so the output goes to the agents console
        // we will need to redirect the output 
        public static string ExecuteAssembly(byte[] asm, string[] arguments = null)
        {
            if (arguments == null)
            {
                arguments = new string[] { };
            }

            var currentOut = Console.Out; // Current agents output stream (Console out)
            var currentError = Console.Error;// now for error

            var ms = new MemoryStream();
            var sw = new StreamWriter(ms)
            {
                AutoFlush = true,
            };


            Console.SetOut(sw); // Change agent output stream to our sw
            Console.SetError(sw);

            var assembly = Assembly.Load(asm);
            assembly.EntryPoint.Invoke(null, new object[] { arguments });

            Console.Out.Flush(); // Clear console
            Console.Error.Flush();

            var output = Encoding.UTF8.GetString(ms.ToArray());

            Console.SetOut(currentOut); // Change back
            Console.SetError(currentError);

            ms.Dispose();  // Free
            sw.Dispose();

            return output;

        }
    }
}