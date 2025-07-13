using Knight.Models.Raven;
using System.IO;

namespace Knight.Commands
{
    public class CreateDirectory : KnightCommand
    {
        public override string Name => "mkdir";

        public override string Execute(TaskMessage task)
        {
            string path;

            //if (task.Arguments is null || task.Arguments.Length == 0)
            if (task.Arguments is null || task.Arguments[0] == "") // args are required
            {
                return "No path provided";
            }
            else
            {
                path = task.Arguments[0];
            }

            var dirInfo = Directory.CreateDirectory(path); // Works for Absolute or Relative

            return $"{dirInfo.FullName} created";
        }
    }
}