using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace liblauncher.libs
{

    [DataContract]
    public class ros
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string name;
        [DataMember(Order = 1, IsRequired = false)]
        public string version;
    }
}
