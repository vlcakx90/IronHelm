using SQLite;

namespace Castle.Interfaces
{
    public interface IDatabaseService
    {
        SQLiteConnection GetConnection();
        SQLiteAsyncConnection GetAsyncConnection();
    }
}
