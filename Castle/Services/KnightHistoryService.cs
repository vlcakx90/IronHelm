using Castle.Dao;
using Castle.Interfaces;
using Castle.Models.Raven;
using Castle.Models.Knight;
using System.Runtime.CompilerServices;

namespace Castle.Services
{
    public class KnightHistoryService : IKnightHistoryService
    {
        private readonly IDatabaseService _databaseService;
        private readonly ILogger<KnightHistoryService> _logger;

        public KnightHistoryService(IDatabaseService databaseService, ILogger<KnightHistoryService> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }


        public async Task Add(KnightHistory knightHistory)
        {
            var conn = _databaseService.GetAsyncConnection();

            var count = await conn.InsertAsync((KnightHistoryDao)knightHistory);
            _logger.LogInformation($"KnightHIstoryService.Add: added {count} rows");
        }

        public async Task<IEnumerable<KnightHistory>> GetAll()
        {
            var conn = _databaseService.GetAsyncConnection();

            var histories = await conn.Table<KnightHistoryDao>().ToArrayAsync();

            return histories.Select(h => (KnightHistory)h);
        }

        public async Task<IEnumerable<KnightHistory>> GetAll(string knightId)
        {
            var conn = _databaseService.GetAsyncConnection();

            var histories = await conn.Table<KnightHistoryDao>().Where(h => h.KnightId == knightId).ToArrayAsync();

            return histories.Select(h => (KnightHistory)h);
        }


        public async Task<KnightHistory> Get(string knightId, string taskId)
        {
            var conn = _databaseService.GetAsyncConnection();

            return await conn.Table<KnightHistoryDao>().FirstOrDefaultAsync(h =>
                h.KnightId == knightId && h.TaskId == taskId);
        }

        public async Task<KnightHistory> GetByTaskId(string taskId)
        {
            var conn = _databaseService.GetAsyncConnection();

            return await conn.Table<KnightHistoryDao>().FirstOrDefaultAsync(s => s.TaskId == taskId);
        }

        public async Task<IEnumerable<KnightHistory>> GetPending(string knightId)
        {
            var conn = _databaseService.GetAsyncConnection();

            var histories = await conn.Table<KnightHistoryDao>()
                .Where(h => h.Status == (int)KNIGHT_TASK_STATUS.PENDING && h.KnightId == knightId)
                .ToArrayAsync();

            return histories.Select(h => (KnightHistory)h);
        }

        public async Task<TaskMessage> GetTask(string knightId)
        {
            throw new NotImplementedException();
        }

        public async Task<TaskResultMessage> GetResult(string knightId)
        {
            throw new NotImplementedException();
        }

        public async Task Update(IEnumerable<KnightHistory> knightHistory)
        {
            var conn = _databaseService.GetAsyncConnection();
            var dao = knightHistory.Select(h => (KnightHistoryDao)h);

            await conn.UpdateAllAsync(dao);
        }

        public async Task Update(KnightHistory knightHistory)
        {
            var conn = _databaseService.GetAsyncConnection();

            await conn.UpdateAsync((KnightHistoryDao)knightHistory);
        }
        public async Task Remove(KnightHistory knightHistory)
        {
            var conn = _databaseService.GetAsyncConnection();

            await conn.DeleteAsync((KnightHistoryDao)knightHistory);
        }
    }
}
