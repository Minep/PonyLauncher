using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace liblauncher
{
    [DataContract]
    public class Config : ICloneable
    {
        [DataMember]
        public string Javaw;
        [DataMember]
        public string Username;
        [DataMember]
        public string Javaxmx;
        [DataMember]
        public string LastPlayVer;
        [DataMember]
        public string ExtraJvmArg;
        [DataMember]
        public string GUID;
        [DataMember]
        public int Height;
        [DataMember]
        public int Width;
        [DataMember]
        public bool FullScreen;
        [DataMember]
        public string MCP;

        public Config()
        {
            Javaw = GetJavaDir() ?? "javaw.exe";
            Username = "NUL";
            Javaxmx = (GetMemory() / 4).ToString(CultureInfo.InvariantCulture);
            ExtraJvmArg = " -Dfml.ignoreInvalidMinecraftCertificates=true -Dfml.ignorePatchDiscrepancies=true";
            GUID = GetGuid();
            Height = -1;
            Width = -1;
            FullScreen = false;
        }

        

        public object Clone()
        {
            return (Config)this.MemberwiseClone();
        }

        public static Config Load(string file)
        {
            if (!File.Exists(file))
                return new Config();
            try
            {
                var fs = new FileStream(file, FileMode.Open);
                var ser = new DataContractSerializer(typeof(Config));
                var cfg = (Config)ser.ReadObject(fs);
                fs.Close();
                if (cfg.GUID == null)
                {
                    cfg.GUID = GetGuid();
                }
                return cfg;
            }
            catch
            {
                MessageBox.Show("加载配置文件遇到错误，使用默认配置");
                return new Config();
            }
        }
        public static void Save(Config cfg = null, string file = null)
        {
            var fs = new FileStream(file, FileMode.Create);
            var ser = new DataContractSerializer(typeof(Config));
            ser.WriteObject(fs, cfg);
            fs.Close();
        }

        public void Save(string file)
        {
            Save(this, file);
        }
        /// <summary>
        /// 读取注册表，寻找安装的java路径
        /// </summary>
        /// <returns>javaw.exe路径</returns>
        public static string GetJavaDir()
        {
            try
            {
                RegistryKey reg = Registry.LocalMachine;
                var openSubKey = reg.OpenSubKey("SOFTWARE");
                if (openSubKey != null)
                {
                    var registryKey = openSubKey.OpenSubKey("JavaSoft");
                    if (registryKey != null)
                        reg = registryKey.OpenSubKey("Java Runtime Environment");
                }
                if (reg != null)
                {
                    List<string> javaList = new List<string>();
                    foreach (string ver in reg.GetSubKeyNames())
                    {
                        try
                        {
                            RegistryKey command = reg.OpenSubKey(ver);
                            if (command != null)
                            {
                                string str = command.GetValue("JavaHome").ToString();
                                if (str != "")
                                    javaList.Add(str + @"\bin\javaw.exe");
                            }
                        }
                        catch { return null; }
                    }
                    //先找出Java7 因为Java8不能正常启动1.7.2
                    foreach (string java in javaList)
                    {
                        if (java.ToLower().Contains("jre7") || java.ToLower().Contains("jdk1.7") || java.ToLower().Contains("jre1.7"))
                        {//可能这样判断版本的方法不太好
                            return java;
                        }
                    }
                    //没有Java7的时候返回第一个Java
                    return javaList[0];
                }
                return null;
            }
            catch { return null; }

        }
        /// <summary>
        /// 获取系统物理内存大小
        /// </summary>
        /// <returns>系统物理内存大小，支持64bit,单位MB</returns>
        public static ulong GetMemory()
        {
            try
            {
                var cimobject1 = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection moc1 = cimobject1.GetInstances();
                double capacity = moc1.Cast<ManagementObject>().Sum(mo1 => ((Math.Round(long.Parse(mo1.Properties["Capacity"].Value.ToString()) / 1024.0 / 1024.0, 1))));
                moc1.Dispose();
                cimobject1.Dispose();
                UInt64 qmem = Convert.ToUInt64(capacity.ToString(CultureInfo.InvariantCulture));
                return qmem;
            }
            catch (Exception ex)
            {
                return ulong.MaxValue;
            }
        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
