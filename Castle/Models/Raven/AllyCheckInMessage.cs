using Castle.Models.Knight;
using System.Runtime.Serialization;

namespace Castle.Models.Raven
{
    [DataContract]
    public class AllyCheckInMessage
    {
        [DataMember(Name = "taskid")]
        public string TaskId { get; set; }

        [DataMember(Name = "parentid")]
        public string ParentId { get; set; }

        [DataMember(Name = "direction")]
        public AllyDirection Direction { get; set; }

        [DataMember(Name = "metadata")]
        public KnightMetadata Metadata { get; set; }


        public AllyCheckInMessage(string taskId, string parentId, AllyDirection direction, KnightMetadata knightMetadata)
        {
            TaskId = taskId;
            ParentId = parentId;
            Direction = direction;
            Metadata = knightMetadata;
        }
    }

    public enum AllyDirection
    {
        TO_CHILD,
        FROM_CHILD,
        NONE
    }
}
