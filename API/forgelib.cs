using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MCLauncher.API
{
    [DataContract]
    public class forgelib
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string branch;
        [DataMember(Order = 1, IsRequired = true)]
        public string build;
        [DataMember(Order = 2, IsRequired = true)]
        public string mcversion;
        [DataMember(Order = 3, IsRequired = true)]
        public string modified;
        [DataMember(Order = 4, IsRequired = true)]
        public string version;
        [DataMember(Order = 5, IsRequired = true)]
        public string _id;
        [DataMember(Order = 6, IsRequired = true)]
        public files[] files;
    }
}
