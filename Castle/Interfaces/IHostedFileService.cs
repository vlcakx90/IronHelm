using Castle.Models.HostedFile;

namespace Castle.Interfaces
{
    public interface IHostedFileService
    {
        Task Add(HostedFile file);
        Task<IEnumerable<HostedFile>> Get();
        Task<HostedFile> Get(string id);
        Task Delete(HostedFile file);
    }
}
