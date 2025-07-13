using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Castle.Models.Raven
{
    [DataContract]
    public class AllyRemoveMessage
    {
        [DataMember(Name = "childid")]
        public string ChildId { get; set; }

        public AllyRemoveMessage(string childId)
        {
            ChildId = childId;
        }
    }
}
