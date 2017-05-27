using System.Runtime.Serialization;

namespace liblauncher.libs
{
    [DataContract]
    public class extract
    {
        [DataMember(Order = 0, IsRequired = false)]
        public string[] exclude;
    }
}
