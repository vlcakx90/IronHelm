using System;
using System.Runtime.Serialization;

namespace Knight.Models.Raven
{
    [DataContract]
    public class TaskResultMessage
    {
        [DataMember(Name = "id")]
        public string TaskId { get; set; }

        [DataMember(Name = "result")]
        public string Result { get; set; }

        [DataMember(Name = "completedat")]
        public DateTime CompletetedAt { get; set; }
    }
}
