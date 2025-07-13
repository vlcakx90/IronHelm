using Castle.Interfaces;

namespace Castle.Models.Commander
{
    public abstract class Commander
    {
        public abstract COMMANDER_TYPE Type { get; }
        public abstract string Name { get; }
        public abstract int BindPort { get; }
        //public abstract string Passwd { get; }
        public abstract bool Tls {  get; }

        protected IKnightService SoldierService;

        protected IRavenService RavenService;

        protected IC2ProfileService C2ProfileService;

        public void Init(IKnightService soldierService, IRavenService ravenService, IC2ProfileService c2ProfileService)
        {
            this.SoldierService = soldierService;
            this.RavenService = ravenService;
            this.C2ProfileService = c2ProfileService;
        }

        public abstract Task Start();
        public abstract void Stop();
    }

    public enum COMMANDER_TYPE
    {
        HTTP,
        //QUIC,
        //WEBSOCKET
        //TCP,
        //SMB,
    }
}
