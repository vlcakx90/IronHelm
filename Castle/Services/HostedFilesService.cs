using Castle.Dao;
using Castle.Interfaces;
using Castle.Models.HostedFile;

namespace Castle.Services
{
    public class HostedFilesService : IHostedFileService
    {
        private readonly IDatabaseService _databaseService;

        public HostedFilesService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }
        public async Task Add(HostedFile file)
        {
            var conn = _databaseService.GetAsyncConnection();
            await conn.InsertAsync((HostedFileDao)file);
        }

        public async Task<IEnumerable<HostedFile>> Get()
        {
            var conn = _databaseService.GetAsyncConnection();
            var files = await conn.Table<HostedFileDao>().ToArrayAsync();

            return files.Select(f => (HostedFile)f);
        }

        public async Task<HostedFile> Get(string id)
        {
            var conn = _databaseService.GetAsyncConnection();

            return await conn.Table<HostedFileDao>().FirstOrDefaultAsync(h => h.Id.Equals(id));
        }

        public async Task Delete(HostedFile file)
        {
            var conn = _databaseService.GetAsyncConnection();
            await conn.DeleteAsync((HostedFileDao)file);
        }
    }
}
