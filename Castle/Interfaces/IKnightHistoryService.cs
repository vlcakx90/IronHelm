using Castle.Models.Raven;
using Castle.Models.Knight;

namespace Castle.Interfaces
{
    public interface IKnightHistoryService
    {
        Task Add(KnightHistory knightHistory);


        Task<IEnumerable<KnightHistory>> GetAll();


        //Task<SoldierHistory> Get(string soldierId);

        Task<IEnumerable<KnightHistory>> GetAll(string soldierId);


        Task<KnightHistory> Get(string soldierId, string taskId);

        Task<KnightHistory> GetByTaskId(string taskId);

        Task<IEnumerable<KnightHistory>> GetPending(string soldierId);


        Task<TaskMessage> GetTask(string soldierId);

        Task<TaskResultMessage> GetResult(string soldierId);


        Task Update(IEnumerable<KnightHistory> knightHistory);
        Task Update(KnightHistory knightHistory);
        Task Remove(KnightHistory knightHistory);
    }
}
