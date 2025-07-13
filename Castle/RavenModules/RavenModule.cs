using Castle.Interfaces;
using Castle.Models.Raven;
using Castle.Services;

namespace Castle.RavenModules
{
    public abstract class RavenModule : IRavenModule
    {
        public abstract RavenType RavenType { get; }

        protected IKnightService KnightService { get; private set;  }
        protected IKnightHistoryService KnightHistoryService { get; private set; }

        protected IPeerToPeerService PeerToPeerService { get; private set; }

        public void Init(RavenService ravenService)
        {
            KnightService = ravenService.KnightService;
            KnightHistoryService = ravenService.KnightHistoryService;
            PeerToPeerService = ravenService.PeerToPeerService;
        }

        public abstract Task ProcessRaven(Raven raven);
    }
}
