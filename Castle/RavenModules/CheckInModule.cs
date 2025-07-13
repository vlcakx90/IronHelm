using Castle.Models.Raven;
using Castle.Utils;

namespace Castle.RavenModules
{
    public class CheckInModule : RavenModule
    {
        public override RavenType RavenType => RavenType.CHECK_IN;

        public override async Task ProcessRaven(Raven raven)
        {
            var checkInMsg = Crypto.Decode<CheckInMessage>(raven.Message);

            //// Look for Soldier
            //var soldier = await SoldierService.GetSoldier(checkInMsg.Metadata.Id);

            //// If not found, make one
            //if (soldier == null)
            //{
            //    soldier = new Models.Soldier.Soldier(checkInMsg.Metadata);

            //    await SoldierService.AddSoldier(soldier);
            //}

            //// Update LastSeen
            //soldier.CheckIn();

            //// Update db
            //await SoldierService.Update(soldier);

            var soldier = await KnightService.GetKnight(checkInMsg.Metadata.Id);

            if (soldier == null)
            {
                //soldier = new Models.Soldier.Soldier(checkInMsg.Metadata);
                soldier = new Models.Knight.Knight(checkInMsg.Metadata, "", AllyDirection.NONE);

                soldier.CheckIn();
                await KnightService.AddKnight(soldier);

                PeerToPeerService.AddVertex(soldier.Metadata.Id);

                var search = PeerToPeerService.DepthFirstSearch(soldier.Metadata.Id);
                Console.WriteLine("== CheckIn");
                PeerToPeerService.PrintVertexes(search);
            }
            else
            {
                soldier.CheckIn();
                await KnightService.Update(soldier);
            }
        }
    }
}
