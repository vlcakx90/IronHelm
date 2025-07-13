using Castle.Models.Commander;

namespace Castle.Interfaces
{
    public interface ICommanderService
    {
        Task LoadFromDb();
        Task AddCommander(Commander commander);
        bool CheckDuplicateCommander(Commander commander);
        IEnumerable<Commander> GetCommanders();
        Commander? GetCommander(string name);
        Task RemoveCommander(Commander commander);
    }
}
