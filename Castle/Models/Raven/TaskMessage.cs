using Castle.Models.Knight;
using System;
using System.Runtime.Serialization;

namespace Castle.Models.Raven
{
    [DataContract]
    public class TaskMessage
    {
        [DataMember(Name = "taskid")]
        public string TaskId { get; set; } = string.Empty;

        [DataMember(Name = "command")]
        public string Command { get; set; } = string.Empty;

        [DataMember(Name = "arguments")]
        public string[] Arguments { get; set; } = new string[0];


        //////////////////////// file as bytes, bytes will be Base64 encoded as String by server

        [DataMember(Name = "file")]
        public byte[] File { get; set; } = new byte[0];

        //[DataMember(Name = "endTime")]
        //public DateTime EndTime { get; set; }

        //[DataMember(Name = "starttime")] // now in SoldierHistory
        //public DateTime StartTime { get; set; }








        /////////// Soldiers version

        //[DataMember(Name = "file")]     
        //public string File { get; set; } 
        //public byte[] FileBytes
        //{
        //    get
        //    {
        //        if (string.IsNullOrEmpty(File))
        //        {
        //            return new byte[0];
        //        }

        //        return Convert.FromBase64String(File);
        //    }
        //}

        public static implicit operator TaskMessage(KnightHistory knightHistory)
        {
            return new TaskMessage
            {
                TaskId = knightHistory.TaskId,
                Command = knightHistory.Command,
                Arguments = knightHistory.Arguments,
                File = knightHistory.File,
            };
        }
    }
}
