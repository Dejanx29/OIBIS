using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string Student { get; set; }
    }
}