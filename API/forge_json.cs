using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace MCLauncher.API
{
    [DataContract]
    class forge_json
    {
        [DataMember(IsRequired = true)]
        public static forgelib[] f;

        public static forgelib[] read(Stream j)
        {
            forgelib[] fj;
            DataContractJsonSerializer InfoReader = new DataContractJsonSerializer(typeof(forge_json));
            fj = InfoReader.ReadObject(j) as forgelib[];
            return fj;
        }
    }
}
