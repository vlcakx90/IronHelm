using Knight.Models.Knight;
using System.Runtime.Serialization;

namespace Knight.Models.Raven
{
    [DataContract]
    public class CheckInMessage
    {
        [DataMember(Name = "metadata")]
        public KnightMetadata Metadata { get; set; }

        public CheckInMessage(KnightMetadata metadata)
        {
            Metadata = metadata;
        }
    }
}
