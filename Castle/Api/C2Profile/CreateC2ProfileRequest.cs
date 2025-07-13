namespace Castle.Api.C2Profile
{
    public class CreateC2ProfileRequest
    {
        // Name (filename)
        public string Name { get; set; } = string.Empty;

        // Http
        public int Sleep { get; set; }
        public int Jitter { get; set; }
        public string Passwd { get; set; } = string.Empty;
        public string[] GetPaths { get; set; } = new string[0];
        public string[] PostPaths { get; set; } = new string[0];
    }
}
