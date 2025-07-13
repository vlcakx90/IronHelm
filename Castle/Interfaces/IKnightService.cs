using Castle.Models.Knight;

namespace Castle.Interfaces
{
    public interface IKnightService
    {
        Task AddKnight(Knight knight);
        Task<IEnumerable<Knight>> GetKnights();
        Task<Knight> GetKnight(string id);

        Task Update(Knight knight);
        Task RemoveKnight(Knight knight);
    }
}
