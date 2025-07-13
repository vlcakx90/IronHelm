using Castle.Models.C2Profile;

namespace Castle.Interfaces
{
    public interface IC2ProfileService
    {
        // C2Profile Properties

        string ProfileName();
        int ProfileHttpSleep();
        int ProfileHttpJitter();
        string ProfileHttpPasswd();
        string[] ProfileHttpGetPaths();
        string[] ProfileHttpPostPaths();
        IEnumerable<string> ProfileHttpAllPaths();


        // C2Profile management
        Task<C2Profile?> SetProfile(string profileFile);

        C2Profile GetCurrentProfile();
        Task<IEnumerable<C2Profile>> GetProfiles();
        Task<C2Profile?> GetProfile(string profileFile);
        Task<bool> CreateProfile(string profileFile, C2Profile profile);
        bool CheckDuplicateProfile(string profileFile);
        //Task<bool> UpdateProfile(string profileFile, C2Profile profile);
        bool DeleteProfile(string profileFile);
    }
}
