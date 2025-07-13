using Castle.Dao;
using Castle.Interfaces;
using SQLite;

namespace Castle.Services
{
    public class DatabaseService : IDatabaseService
    {
        //private const string DB_PATH = @"C:\Users\Leafy\source\repos\LevitatingLeafy\IronHelm\Castle";
        private const string DB_DIR =   "Storage";
        private const string DB_FILE =  "IronHelm.db";

        private readonly SQLiteConnection _connection;
        private readonly SQLiteAsyncConnection _asyncConnection;

        public DatabaseService()
        {
            //string path = Path.Combine(DB_PATH, DB_FILE);
            //string targetDir = Path.Combine(DB_PATH, DB_DIR);

            string targetDir = Path.Combine(Directory.GetCurrentDirectory(), DB_DIR);

            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir); 
            }

            string path = Path.Combine(targetDir, DB_FILE);


            using (SQLiteConnection conn = new SQLiteConnection(path))
            {
                // Users
                conn.CreateTable<UserDao>();
                // Commanders
                conn.CreateTable<HttpCommanderDao>();
                // Soldier
                conn.CreateTable<KnightDao>();
                // soldier history
                conn.CreateTable<KnightHistoryDao>();
                // hosted files
                conn.CreateTable<HostedFileDao>();
            }

            _connection = new SQLiteConnection(path);
            _asyncConnection = new SQLiteAsyncConnection(path);
        }
        public SQLiteAsyncConnection GetAsyncConnection()
        {
            return _asyncConnection;
        }

        public SQLiteConnection GetConnection()
        {
            return _connection;
        }
    }
}
