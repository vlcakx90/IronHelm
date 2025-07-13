namespace Castle.Models.C2Profile
{
    public sealed class C2Profile
    {
        public string Name { get; set; } = string.Empty; // same as file name
        public HttpProfile Http { get; set; } = new HttpProfile();

        public sealed class HttpProfile
        {
            public int Sleep { get; set; } = 2;// interval
            public int Jitter { get; set; } = 1;

            public string Passwd { get; set; } = "HelmOfIron";
            public string[] GetPaths { get; set; } = { "/index.php" };
            public string[] PostPaths { get; set; } = { "/submit.php" };
        }
    }
}
