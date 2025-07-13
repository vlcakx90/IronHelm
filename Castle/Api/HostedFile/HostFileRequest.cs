namespace Castle.Api.HostedFile
{
    public class HostFileRequest
    {
        public string Commander { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public byte[] Bytes { get; set; } = new byte[0];
    }
}
