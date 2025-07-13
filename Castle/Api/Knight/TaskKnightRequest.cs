namespace Castle.Api.Knight
{
    public class TaskKnightRequest
    {
        public string Command { get; set; } = string.Empty;
        public string[] Arguments { get; set; } = new string[0];
        public byte[] File { get; set; } = new byte[0];
    }
}
