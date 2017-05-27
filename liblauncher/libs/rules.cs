using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace liblauncher.libs
{
    [DataContract]
    public class rules
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string action;
        [DataMember(Order = 1, IsRequired = false)]
        public ros os;
        [DataMember(Order = 2, IsRequired = false)]
        public string version;
    }

}
