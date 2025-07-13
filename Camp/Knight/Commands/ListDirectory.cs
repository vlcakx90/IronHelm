using SharpSploit.Generic;
using Knight.Models.Raven;
using System.Collections.Generic;
using System.IO;

namespace Knight.Commands
{
    public class ListDirectory : KnightCommand
    {
        public override string Name => "ls";

        public override string Execute(TaskMessage task)
        {
            var results = new SharpSploitResultList<ListDirectoryResult>();
            string path;

            if (task.Arguments is null || task.Arguments.Length == 0)
            //if (task.Arguments is null || task.Arguments[0] == "") // needed when using through swagger
            {
                path = Directory.GetCurrentDirectory();
            }
            else
            {
                path = task.Arguments[0];
            }

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                results.Add(new ListDirectoryResult
                {
                    Name = fileInfo.FullName,
                    Length = fileInfo.Length, // Can add more properties as you see fit
                });
            }

            var directories = Directory.GetDirectories(path);

            foreach (var d in directories)
            {
                var dirInfo = new DirectoryInfo(d);
                results.Add(new ListDirectoryResult
                {
                    Name = dirInfo.FullName,
                    Length = 0
                });
            }

            return results.ToString();
        }
    }


    // Add properties and IList override for Generics from SharpSploit
    public sealed class ListDirectoryResult : SharpSploitResult
    {
        public string Name { get; set; }

        public long Length { get; set; }

        protected internal override IList<SharpSploitResultProperty> ResultProperties => new List<SharpSploitResultProperty>
        {
            new SharpSploitResultProperty { Name = nameof(Name), Value = Name },
            new SharpSploitResultProperty { Name = nameof(Length), Value = Length }
        };

    }
}