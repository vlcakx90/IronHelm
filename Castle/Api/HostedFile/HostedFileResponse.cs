namespace Castle.Api.HostedFile
{
    public class HostedFileResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Commander { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}
