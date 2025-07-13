using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Knight.Models.Raven
{
    [DataContract]
    public class HelloChildMessage // Parent to Child
    {
        [DataMember(Name = "taskid")]
        public string TaskId { get; set; }

        [DataMember(Name = "parentid")]
        public string ParentId { get; set; }

        [DataMember(Name = "direction")]
        public AllyDirection Direction { get; set; }

        public HelloChildMessage(string taskId, string parentId, AllyDirection direction)
        {
            TaskId = taskId;
            ParentId = parentId;
            Direction = direction;
        }
    }
}
