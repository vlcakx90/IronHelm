namespace Castle.Api.Commander
{
    public class StartHttpCommanderRequest
    {
        public string Name { get; set; } = string.Empty;
        public int BindPort { get; set; }

        //public string Passwd { get; set; } = string.Empty;
        public bool Tls { get; set; }
    }
}
