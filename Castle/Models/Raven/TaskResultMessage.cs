using System;
using System.Runtime.Serialization;

namespace Castle.Models.Raven
{
    [DataContract]
    public class TaskResultMessage
    {
        [DataMember(Name = "id")]
        public string TaskId { get; set; } = string.Empty;

        [DataMember(Name = "result")]
        public string Result { get; set; } = string.Empty;

        [DataMember(Name = "completedat")]
        public DateTime CompletetedAt { get; set; }
    }
}
