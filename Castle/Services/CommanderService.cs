using Castle.Dao;
using Castle.Interfaces;
using Castle.Models.Commander;
using SQLite;

namespace Castle.Services
{
    public class CommanderService : ICommanderService
    {
        private readonly IDatabaseService _databaseService;
        private readonly IKnightService _soldierService;
        private readonly IRavenService _ravenService;
        private readonly IC2ProfileService _c2ProfileService;

        private readonly List<Commander> _commanders = new List<Commander>();

        public CommanderService(IDatabaseService databaseService, IKnightService soldierService, IRavenService ravenService, IC2ProfileService c2ProfileService)
        {
            _databaseService = databaseService;
            _soldierService = soldierService;
            _ravenService = ravenService;
            _c2ProfileService = c2ProfileService;
        }

        public async Task LoadFromDb()
        {
            ///// Skip Db read, make defaults
            Commander commander= new HttpCommander("http-1", 8080, false);
            commander.Init(_soldierService, _ravenService, _c2ProfileService); // Problem is we need to pass the _agentService var from ListenersController -> now fixed with DI above
            commander.Start();
            if (!CheckDuplicateCommander(commander))
            {
                _commanders.Add(commander);
            }

            commander = new HttpCommander("http-2", 8443, true);
            commander.Init(_soldierService, _ravenService, _c2ProfileService); // Problem is we need to pass the _agentService var from ListenersController -> now fixed with DI above
            commander.Start();
            if (!CheckDuplicateCommander(commander))
            {
                _commanders.Add(commander);
            }
            //////

            /////////// DB Read
            //SQLiteAsyncConnection conn = _databaseService.GetAsyncConnection();
            //var httpCommanders = await conn.Table<HttpCommanderDao>().ToListAsync();

            //foreach (var c in httpCommanders)
            //{
            //    Commander commander = new HttpCommander(c.Name, c.BindPort, c.Tls);
            //    commander.Init(_soldierService, _ravenService, _c2ProfileService);
            //    commander.Start(); // LEAVE SYNCHRONOUS
            //    _commanders.Add(commander);
            //}
        }
        public async Task AddCommander(Commander commander)
        {
            _commanders.Add(commander);

            SQLiteAsyncConnection conn = _databaseService.GetAsyncConnection();

            switch (commander.Type)
            {
                case (COMMANDER_TYPE.HTTP):
                    await conn.InsertAsync((HttpCommanderDao)commander);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(commander.Type)}");
            }
        }

        public bool CheckDuplicateCommander(Commander commander)
        {
            Commander? l = _commanders.Find(l => l.BindPort.Equals(commander.BindPort));
            if (l is not null)
            {
                return true;
            }

            return false;

            //return _listeners.Contains(listener);
        }

        public IEnumerable<Commander> GetCommanders()
        {
            return _commanders;
        }
        public Commander? GetCommander(string name)
        {
            return GetCommanders().FirstOrDefault(c => c.Name.Equals(name));
        }

        public async Task RemoveCommander(Commander commander)
        {
            _commanders.Remove(commander);

            SQLiteAsyncConnection conn = _databaseService.GetAsyncConnection();

            switch (commander.Type)
            {
                case (COMMANDER_TYPE.HTTP):
                    await conn.DeleteAsync((HttpCommanderDao)commander);
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nameof(commander.Type)}");
            }
        }
    }
}
