using System;
using System.Runtime.Serialization;

namespace Knight.Models.Raven
{
    [DataContract]
    public class TaskMessage
    {
        [DataMember(Name = "taskid")]
        public string TaskId { get; set; }

        [DataMember(Name = "command")]
        public string Command { get; set; }

        [DataMember(Name = "arguments")]
        public string[] Arguments { get; set; }

        //[DataMember(Name = "endTime")]
        //public DateTime EndTime { get; set; }

        [DataMember(Name = "file")]     // Maybe this can be a byte[] now since this whole object will be insidee a Rave (5/27)
        public byte[] File { get; set; }
        //public string File { get; set; } // Changed to from byte[] to string (TeamServer serializer converts byte[] into base64 string, so the DataContractJsonSerializer here fails
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
    }
}
