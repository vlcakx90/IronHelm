using Castle.Dao;
using Castle.Interfaces;
using Castle.Models.Knight;

namespace Castle.Services
{
    public class KnightService : IKnightService
    {
        private readonly IDatabaseService _databaseService;

        private readonly List<Knight> _knights = new List<Knight>();

        public KnightService(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task AddKnight(Knight knight)
        {
            //_knights.Add(agent);
            var conn = _databaseService.GetAsyncConnection();

            await conn.InsertAsync((KnightDao)knight);
        }

        public async Task<IEnumerable<Knight>> GetKnights()
        {
            //return _knights;
            var conn = _databaseService.GetAsyncConnection();
            var knights = await conn.Table<KnightDao>().ToArrayAsync();

            return knights.Select(s => (Knight)s);
        }
        public async Task<Knight> GetKnight(string id)
        {
            //return GetKnights().FirstOrDefault(s => s.Metadata.Id.Equals(id));
            var conn = _databaseService.GetAsyncConnection();

            return await conn.Table<KnightDao>().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task Update(Knight knight)
        {
            var conn = _databaseService.GetAsyncConnection();

            await conn.UpdateAsync((KnightDao)knight);
        }
        public async Task RemoveKnight(Knight knight)
        {
            //_knights.Remove(knight);
            var conn = _databaseService.GetAsyncConnection();

            await conn.DeleteAsync((KnightDao)knight);
        }
    }
}
