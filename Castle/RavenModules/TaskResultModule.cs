using Castle.Models.Raven;
using Castle.Utils;

namespace Castle.RavenModules
{
    public class TaskResultModule : RavenModule
    {
        public override RavenType RavenType => RavenType.TASK_RESULT;

        public override async Task ProcessRaven(Raven raven)
        {
            var taskResultMsg = Crypto.Decode<TaskResultMessage>(raven.Message);

            // Find it Task exists in history table
            var history = await KnightHistoryService.GetByTaskId(taskResultMsg.TaskId);

            // Not Found
            if (history == null)
            {
                return;
            }

            // Update with results
            history.Update(taskResultMsg);

            // Update db
            await KnightHistoryService.Update(history);
        }
    }
}
