using Castle.Api.HostedFile;

namespace Castle.Models.HostedFile
{
    public class HostedFile
    {
        public string Id { get; set; } = string.Empty;
        public string Commander { get; set; } = string.Empty;
        public string Uri { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public long Size { get; set; }

        public static implicit operator HostedFile(HostFileRequest request)
        {
            return new HostedFile
            {
                Id = Guid.NewGuid().ToString(),
                Commander = request.Commander,
                Uri = request.Uri,
                Filename = request.Filename,
                Size = request.Bytes.LongLength
            };
        }

        public static implicit operator HostedFileResponse(HostedFile file)
        {
            return new HostedFileResponse
            {
                Id = file.Id,
                Commander = file.Commander,
                Uri = file.Uri,
                Filename = file.Filename,
                Size = file.Size
            };
        }
    }
}
