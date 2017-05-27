using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace liblauncher.libs
{
    [DataContract]
    public class libraryies
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string name;
        [DataMember(Order = 1, IsRequired = false)]
        public OS natives;
        [DataMember(Order = 2, IsRequired = false)]
        public extract extract;
        [DataMember(IsRequired = false)]
        public string url;
        [DataMember(Order = 4, IsRequired = false)]
        public rules[] rules;
    }
}
