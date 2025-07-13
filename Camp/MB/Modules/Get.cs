using System.Net;
using System;
using System.Reflection;
using System.IO;
using System.Text;

namespace MB1.Modules
{
    public static class Get
    {
        public static bool Download(out byte[] data, string url)
        {
            data = null;

            try
            {
                WebClient client = new WebClient();
                data = client.DownloadData(url);
                return true;
            }
            catch (Exception ex)
            {
                data = null;
                return false;
            }
        }

        public static bool ReadFromDisk(out byte[] data, string name)
        {
            data = null;

            try
            {
                data = System.IO.File.ReadAllBytes(name);
                return true;
            }
            catch (Exception ex)
            {
                data = null;
                return false;
            }
        }

        public static bool EmbeddedResource(out byte[] data, string name)
        {
            data= null;

            try
            {
                string found = FindResource(name);
                string resource = GetResourceData(found);
                data = Encoding.UTF8.GetBytes(resource);
                return true;
            }
            catch (Exception ex)
            {
                data = null;
                return false;
            }
        }

        private static string FindResource(string resourceName) // No Linq
        {
            var asm = Assembly.GetExecutingAssembly();
            var resourceNames = asm.GetManifestResourceNames();
            string resource = string.Empty;

            foreach (var name in resourceNames)
            {
                if (name.EndsWith(resourceName))
                {
                    resource = name;
                }
            }

            return resource;
        }
        private static string GetResourceData(string resourcePath) // Requires Namespace (ex. EmbeddedResouce.Resources.Name)
        {
            var asm = Assembly.GetExecutingAssembly();
            string resource = string.Empty;

            using (Stream stream = asm.GetManifestResourceStream(resourcePath))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    resource = sr.ReadToEnd();
                }
            }

            return resource;
        }

    }
}