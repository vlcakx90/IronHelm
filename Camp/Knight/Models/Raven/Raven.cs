using System.Runtime.Serialization;

namespace Knight.Models.Raven
{
    [DataContract]
    public class Raven
    {
        [DataMember(Name = "soldierid")]
        public string SoldierId { get; set; }

        [DataMember(Name = "raventype")]
        public RavenType Type { get; set; }

        [DataMember(Name = "message")] // base64 string of raw bytes
        public string Message { get; set; }

        //[DataMember(Name = "message")] // raw bytes
        //public byte[] Message { get; set; }

        public Raven(string soldierId, RavenType type, string msg = "")
        {
            SoldierId = soldierId;
            Type = type;
            Message = msg;
        }

        public Raven()
        {

        }
    }

    public enum RavenType
    {
        CHECK_IN,      // Egress Soldier to TeamServer
        ALLY_CHECK_IN, // Child to TeamServer via Parent
        HELLO_CHILD,   // Parent to Child

        TASK,
        TASK_RESULT,

        ALLY_REMOVE
    }
}
