using Castle.Models.Raven;
using Castle.Utils;

namespace Castle.RavenModules
{
    public class AllyCheckInModule : RavenModule
    {
        public override RavenType RavenType => RavenType.ALLY_CHECK_IN;

        public override async Task ProcessRaven(Raven raven)
        {
            var allyCheckInMsg = Crypto.Decode<AllyCheckInMessage>(raven.Message);

            // IMPLEMENT P2P CHECKIN

            // Normal Checkin
            var soldier = await KnightService.GetKnight(allyCheckInMsg.Metadata.Id);

            if (soldier == null)
            {
                //soldier = new Models.Soldier.Soldier(allyCheckInMsg.Metadata);
                soldier = new Models.Knight.Knight(allyCheckInMsg.Metadata, allyCheckInMsg.ParentId, allyCheckInMsg.Direction);

                soldier.CheckIn();
                await KnightService.AddKnight(soldier);

                // add vertex and edge to P2P
                PeerToPeerService.AddVertex(soldier.Metadata.Id);
                PeerToPeerService.AddEdge(soldier.ParentId, soldier.Metadata.Id);

                var search = PeerToPeerService.DepthFirstSearch(soldier.Metadata.Id);
                Console.WriteLine("== AllyCheckIn");
                PeerToPeerService.PrintVertexes(search);
                
            }
            else
            {
                soldier.CheckIn(); // needed?? already checkedin inside of RavenService...
                await KnightService.Update(soldier);
            }
        }
    }
}
