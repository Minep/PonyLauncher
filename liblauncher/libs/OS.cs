using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace liblauncher.libs
{
    [DataContract]
    public class OS
    {
        [DataMember(Order = 0, IsRequired = false)]
        public string windows;
        [DataMember(Order = 1, IsRequired = false)]
        public string linux;
        [DataMember(Order = 2, IsRequired = false)]
        public string oxs;
    }
}
