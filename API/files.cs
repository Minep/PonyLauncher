using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace MCLauncher.API
{
    [DataContract]
    public class files
    {
        [DataMember(Order = 0, IsRequired = true)]
        public string format;
        [DataMember(Order = 1, IsRequired = true)]
        public string category;
        [DataMember(Order = 2, IsRequired = true)]
        public string hash;
        [DataMember(Order = 3, IsRequired = true)]
        public string _id;
    }
}
