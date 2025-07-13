using Castle.Interfaces;
using Castle.Models.C2Profile;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Castle.Services
{
    public class C2ProfileService : IC2ProfileService
    {
        private C2Profile _c2Profile;
        private string? _profileDir = null;


        private const string C2PROFILE_DIR = "C2Profiles";
        private const string EXT = ".yaml";

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
        public async Task<C2Profile?> SetProfile(string name)
        {
            string targetDir = Path.Combine(Directory.GetCurrentDirectory(), C2PROFILE_DIR);
            if (!Directory.Exists(targetDir))
            {
                return null;
            }

            string filePath = Path.Combine(targetDir, name + EXT);
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var serializer = new YamlDotNet.Serialization.Deserializer();
                var yaml = await File.ReadAllTextAsync(filePath);
                var profile = serializer.Deserialize<C2Profile>(yaml);
                
                _c2Profile = profile;
                _profileDir = targetDir;

                // DEBUG
                Console.WriteLine($"### SetProfile::Profile Filename:{name} Sleep:{_c2Profile.Http.Sleep} Jitter:{_c2Profile.Http.Jitter} ");

                return _c2Profile;
            }
            catch
            {
                return null;
            }
        }

        public async Task<C2Profile?> SetProfileHardCoded(string name)
        {
            string targetDir = "C:\\Users\\vlcak\\source\\repos\\IronHelm\\Castle\\C2Profiles";            if (!Directory.Exists(targetDir))
            {
                return null;
            }

            string filePath = Path.Combine(targetDir, name + EXT);
            if (!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var serializer = new YamlDotNet.Serialization.Deserializer();
                var yaml = await File.ReadAllTextAsync(filePath);
                var profile = serializer.Deserialize<C2Profile>(yaml);

                _c2Profile = profile;
                _profileDir = targetDir;

                // DEBUG
                Console.WriteLine($"### SetProfile::Profile Filename:{name} Sleep:{_c2Profile.Http.Sleep} Jitter:{_c2Profile.Http.Jitter} ");

                return _c2Profile;
            }
            catch
            {
                return null;
            }
        }
        public C2Profile GetCurrentProfile()
        {
            return _c2Profile;
        }

        // GetAllProfiles()
        public async Task<IEnumerable<C2Profile>> GetProfiles()
        {
            List<C2Profile> profiles = new List<C2Profile>();
            
            if (_profileDir != null)
            {
                var files = Directory.GetFiles(_profileDir, "*.yaml");
                var serializer = new YamlDotNet.Serialization.Deserializer();
                
                foreach (var file in files)
                {
                    try
                    {
                        var yaml = await File.ReadAllTextAsync(file);
                        var profile = serializer.Deserialize<C2Profile>(yaml);

                        profiles.Add(profile);
                    }
                    catch
                    {

                    }
                }
            }

            return profiles;
        }

        // GetProfile(string filename)
        public async Task<C2Profile?> GetProfile(string name)
        {
            C2Profile? profile = new C2Profile();

            if (_profileDir != null)
            {
                string filePath = Path.Combine(_profileDir, name + EXT);
                var serializer = new YamlDotNet.Serialization.Deserializer();

                try
                {
                    var yaml = await File.ReadAllTextAsync(filePath);
                    profile = serializer.Deserialize<C2Profile>(yaml);                    
                }
                catch
                {
                    profile = null;
                }
            }

            return profile;
        }

        // CreateProfile(C2Profile profile) will serialize to yaml and write the file to C2Profiles/
        public async Task<bool> CreateProfile(string name, C2Profile profile)
        {
            if (_profileDir != null)
            {
                string filePath = Path.Combine(_profileDir, name + EXT);

                var serializer = new YamlDotNet.Serialization.Serializer();
                var yaml = serializer.Serialize(profile);

                try
                {
                    await File.WriteAllTextAsync(filePath, yaml);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        public bool CheckDuplicateProfile(string name)
        {
            if (_profileDir != null)
            {
                string filePath = Path.Combine(_profileDir, name + EXT);

                if (File.Exists(filePath))
                {
                    return true;
                }

                return false;
            }

            return false;
        }
        //public async Task<bool> UpdateProfile(string profileFile, C2Profile profile)
        //{
        //    if (_profileDir != null)
        //    {
        //        string filePath = Path.Combine(_profileDir, profileFile);

        //        var serializer = new YamlDotNet.Serialization.Serializer();
        //        var yaml = serializer.Serialize(profile);

        //        try
        //        {
        //            await File.WriteAllTextAsync(filePath, yaml);

        //            return true;
        //        }
        //        catch
        //        {
        //            return false;
        //        }
        //    }

        //    return false;
        //}


        // Delete(string filename) delete the file
        public bool DeleteProfile(string name)
        {
            if (_profileDir != null)
            {
                string filePath = Path.Combine(_profileDir, name + EXT);

                try
                { 
                    File.Delete(filePath);

                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }
    }
}
