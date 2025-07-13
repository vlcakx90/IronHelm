using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CodeBuilder
{
    public class C2ProfileService
    {
        private C2Profile _c2Profile;
        private string? _profileDir = null;


        //private const string C2PROFILE_DIR = "C2Profiles";
        //private const string EXT = ".yaml";

        public C2ProfileService()
        {
            _c2Profile = new C2Profile();
        }

        public string ProfileName()
        {
            return _c2Profile.Name;
        }

        public int ProfileHttpSleep()
        {
            return _c2Profile.Http.Sleep;
        }

        public int ProfileHttpJitter()
        {
            return _c2Profile.Http.Jitter;
        }

        public string ProfileHttpPasswd()
        {
            return _c2Profile.Http.Passwd;
        }

        public string[] ProfileHttpGetPaths()
        {
            return _c2Profile.Http.GetPaths;
        }

        public string[] ProfileHttpPostPaths()
        {
            return _c2Profile.Http.PostPaths;
        }

        public IEnumerable<string> ProfileHttpAllPaths()
        {
            return _c2Profile.Http.GetPaths.Concat(_c2Profile.Http.PostPaths);
        }



        // SetProfile(string filename) get profile path at startup and pass here to load
        public async Task<C2Profile?> SetProfile(string filePath)
        {
            //string targetDir = Path.Combine(Directory.GetCurrentDirectory(), C2PROFILE_DIR);
            //if (!Directory.Exists(targetDir))
            //{
            //    return null;
            //}

            //string filePath = "C:\\users\\vlcak\\source\\repos\\SharpTools\\Libraries\\CodeBuilder\\test.yaml";
            //string filePath = Path.Combine(targetDir, name + EXT);
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var serializer = new YamlDotNet.Serialization.Deserializer();
                var yaml = await File.ReadAllTextAsync(filePath);
                _c2Profile = serializer.Deserialize<C2Profile>(yaml);

                return _c2Profile;
            }
            catch
            {
                return null;
            }
        }

        public void PrintCurrentProfile()
        {
            Console.WriteLine(
                $"Name: {_c2Profile.Name}\n" +
                $"Description: {_c2Profile.Description}\n" +
                $"Http\n" +
                $"  Ip:  {_c2Profile.Http.Ip}\n" +
                $"  Port:  {_c2Profile.Http.Port}\n" +
                $"  Tls:  {_c2Profile.Http.Tls}\n" +
                $"  Sleep:  {_c2Profile.Http.Sleep}\n" +
                $"  Jitter: {_c2Profile.Http.Jitter}\n" +
                $"  PasswdHeader: {_c2Profile.Http.PasswdHeader}\n" +
                $"  Passwd:       {_c2Profile.Http.Passwd}\n" +
                $"  MetadataHeader: {_c2Profile.Http.MetadataHeader}\n" +
                $"  MetadataValue:  {_c2Profile.Http.MetadataValue}\n" +
                //$"  GetPaths[0]:  {_c2Profile.Http.GetPaths[0]}\n" +
                //$"  PostPaths[0]: {_c2Profile.Http.PostPaths[0]}\n" 
                $"SMB\n" +
                $"  Pipename: {_c2Profile.SMB.Pipename}\n" +
                $"TCP\n" +
                $"  Loopack:  {_c2Profile.TCP.Loopback}\n" +
                $"  Port:     {_c2Profile.TCP.Port}\n" +
                $"SharedArsenal\n" +
                $"  ResourceName: {_c2Profile.SharedArsenal.ResourceName}\n" +
                $"MB_Staged\n" +
                $"  Get:     {_c2Profile.MB_Staged.Get}\n" +
                $"  GetUri:  {_c2Profile.MB_Staged.GetUri}\n" +
                $"  Format:  {_c2Profile.MB_Staged.Format}\n" +
                $"  Execute: {_c2Profile.MB_Staged.Execute}\n" +
                $"  CleanUp: {_c2Profile.MB_Staged.CleanUp}\n" +
                $"MB_Stageless\n" +
                $"  Get:     {_c2Profile.MB_Stageless.Get}\n" +
                $"  GetName: {_c2Profile.MB_Stageless.GetName}\n" +
                $"  Format:  {_c2Profile.MB_Stageless.Format}\n" +
                $"  Execute: {_c2Profile.MB_Stageless.Execute}\n" +
                $"  CleanUp: {_c2Profile.MB_Stageless.CleanUp}\n"
                );
        }
    }
}