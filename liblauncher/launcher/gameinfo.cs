using liblauncher.libs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace liblauncher.launcher
{
    [DataContract]
    public class gameinfo : ICloneable
    {
        [DataMember(IsRequired = true)]
        public string id = "";
        [DataMember(IsRequired = false)]
        public string time = "";
        [DataMember(IsRequired = false)]
        public string releaseTime = "";
        [DataMember(IsRequired = false)]
        public string type = "";
        [DataMember(IsRequired = true)]
        public string minecraftArguments = "";
        [DataMember(IsRequired = true)]
        public string mainClass = "";
        [DataMember(IsRequired = true)]
        public libraryies[] libraries = null;
        [DataMember(IsRequired = false)]
        public int minimumLauncherVersion = 0;
        [DataMember(IsRequired = false)]
        public string assets;
        [DataMember(IsRequired = false)]
        public string inheritsFrom = "";
        [DataMember(IsRequired = false)]
        public string jar = "";
        object ICloneable.Clone()
        {
            return this.clone();
        }
        public gameinfo clone()
        {
            return (gameinfo)this.MemberwiseClone();
        }

        static public void Write(gameinfo info, string path)
        {
            DataContractJsonSerializer j = new DataContractJsonSerializer(typeof(gameinfo));
            FileStream fs = new FileStream(path, FileMode.Create);
            j.WriteObject(fs, info);
            fs.Close();
        }

        static public gameinfo Read(string path)
        {
            try
            {
                gameinfo info;
                StreamReader JsonFile = new StreamReader(path);
                DataContractJsonSerializer InfoReader = new DataContractJsonSerializer(typeof(gameinfo));
                info = InfoReader.ReadObject(JsonFile.BaseStream) as gameinfo;
                JsonFile.Close();
                if (info.inheritsFrom == "" || info.inheritsFrom == null)
                    return info;
                String anotherJson = GetGameInfoJsonPath(info.inheritsFrom);
                gameinfo anotherGameinfo = Read(anotherJson);
                info.libraries = MixLibraries(info.libraries, anotherGameinfo.libraries);
                return info;
            }
            catch (SerializationException ex)
            {
                try
                {
                    StreamReader JsonFile = new StreamReader(path, Encoding.Default);
                    DataContractJsonSerializer InfoReader = new DataContractJsonSerializer(typeof(gameinfo));
                    gameinfo info = InfoReader.ReadObject(JsonFile.BaseStream) as gameinfo;
                    JsonFile.Close();
                    JsonFile = new StreamReader(path, Encoding.Default);
                    string JsonString = JsonFile.ReadToEnd();
                    JsonFile.Close();
                    StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8);
                    sw.WriteLine(JsonString);
                    sw.Close();
                    if (info.inheritsFrom == "" || info.inheritsFrom == null)
                        return info;
                    String anotherJson = GetGameInfoJsonPath(info.inheritsFrom);
                    gameinfo anotherGameinfo = Read(anotherJson);
                    info.libraries = MixLibraries(info.libraries, anotherGameinfo.libraries);
                    return info;
                }
                catch (SerializationException ex1)
                {
                    return null;
                }
            }
        }

        static public string GetGameInfoJsonPath(string version)
        {
            StringBuilder JsonFilePath = new StringBuilder();
            JsonFilePath.Append(Core.Config.MCP + @"\.minecraft\versions\");
            JsonFilePath.Append(version);
            JsonFilePath.Append(@"\");
            JsonFilePath.Append(version);
            JsonFilePath.Append(".json");
            if (!File.Exists(JsonFilePath.ToString()))
            {
                DirectoryInfo mcpath = new DirectoryInfo(System.IO.Path.GetDirectoryName(JsonFilePath.ToString()));
                bool find = false;
                foreach (FileInfo js in mcpath.GetFiles())
                {
                    if (js.FullName.EndsWith(".json"))
                    {
                        if (Read(js.FullName) != null)
                        {
                            JsonFilePath = new StringBuilder(js.FullName);
                            find = true;
                        }
                    }
                }
                if (!find)
                {
                    return null;
                }
                else
                {
                    return JsonFilePath.ToString();
                }
            }
            else
            {
                return JsonFilePath.ToString();
            }

        }

        private static libraryies[] MixLibraries(libraryies[] lib1, libraryies[] lib2)
        {
            return lib1.Concat(lib2).ToArray();
        }
    }
}
