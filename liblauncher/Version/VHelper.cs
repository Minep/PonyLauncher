using liblauncher.launcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace liblauncher.Version
{
    class VHelper
    {
        public delegate void ImportProgressChangeEventHandler(string status);
        public static event ImportProgressChangeEventHandler ImportProgressChangeEvent;

        public delegate void ImportFinishEventHandler();
        public static event ImportFinishEventHandler ImportFinish;

        private static void OnImportFinish()
        {
            ImportFinishEventHandler handler = ImportFinish;
        }


        private static void OnImportProgressChangeEvent(string status)
        {
            ImportProgressChangeEventHandler handler = ImportProgressChangeEvent;
        }

        public static void ImportOldMc(string importName, string importFrom, Delegate callback = null)
        {
            var thread = new Thread(() =>
            {
                OnImportProgressChangeEvent("导入Minecraft主程序...");
                Directory.CreateDirectory(".minecraft\\versions\\" + importName);
                File.Copy(importFrom + "\\bin\\minecraft.jar",
                    ".minecraft\\versions\\" + importName + "\\" + importName + ".jar");
                OnImportProgressChangeEvent("创建Json");
                var info = new gameinfo { id = importName };
                string timezone = DateTimeOffset.Now.Offset.ToString();
                if (timezone[0] != '-')
                {
                    timezone = "+" + timezone;
                }
                info.time = DateTime.Now.GetDateTimeFormats('s')[0] + timezone;
                info.releaseTime = DateTime.Now.GetDateTimeFormats('s')[0] + timezone;
                info.type = "Port By BMCL";
                info.minecraftArguments = "${auth_player_name}";
                info.mainClass = "net.minecraft.client.Minecraft";
                OnImportProgressChangeEvent("处理Native");
                var libs = new ArrayList();
                var bin = new DirectoryInfo(importFrom + "\\bin");
                foreach (FileInfo file in bin.GetFiles("*.jar"))
                {
                    var libfile = new libs.libraryies();
                    if (file.Name == "minecraft.jar")
                        continue;
                    if (
                        !Directory.Exists(".minecraft\\libraries\\" + importName + "\\" +
                                          file.Name.Substring(0, file.Name.Length - 4) + "\\BMCL\\"))
                    {
                        Directory.CreateDirectory(".minecraft\\libraries\\" + importName + "\\" +
                                                  file.Name.Substring(0, file.Name.Length - 4) + "\\BMCL\\");
                    }
                    File.Copy(file.FullName,
                        ".minecraft\\libraries\\" + importName + "\\" + file.Name.Substring(0, file.Name.Length - 4) +
                        "\\BMCL\\" + file.Name.Substring(0, file.Name.Length - 4) + "-BMCL.jar");
                    libfile.name = importName + ":" + file.Name.Substring(0, file.Name.Length - 4) + ":BMCL";
                    libs.Add(libfile);
                }
                var nativejar = new ICSharpCode.SharpZipLib.Zip.FastZip();
                if (!Directory.Exists(".minecraft\\libraries\\" + importName + "\\BMCL\\"))
                {
                    Directory.CreateDirectory(".minecraft\\libraries\\" + importName + "\\native\\BMCL\\");
                }
                nativejar.CreateZip(
                    ".minecraft\\libraries\\" + importName + "\\native\\BMCL\\native-BMCL-natives-windows.jar",
                    importFrom + "\\bin\\natives", false, @"\.dll");
                var nativefile = new libs.libraryies { name = importName + ":native:BMCL" };
                var nativeos = new libs.OS { windows = "natives-windows" };
                nativefile.natives = nativeos;
                nativefile.extract = new libs.extract();
                libs.Add(nativefile);
                info.libraries = (libs.libraryies[])libs.ToArray(typeof(libs.libraryies));
                OnImportProgressChangeEvent("写入Json");
                var wcfg = new FileStream(".minecraft\\versions\\" + importName + "\\" + importName + ".json",
                    FileMode.Create);
                var infojson = new DataContractJsonSerializer(typeof(gameinfo));
                infojson.WriteObject(wcfg, info);
                wcfg.Close();
                OnImportProgressChangeEvent("处理库文件");
                if (Directory.Exists(importFrom + "\\lib"))
                {
                    if (!Directory.Exists(".minecraft\\lib"))
                    {
                        Directory.CreateDirectory(".minecraft\\lib");
                    }
                    foreach (
                        string libfile in Directory.GetFiles(importFrom + "\\lib", "*", SearchOption.AllDirectories))
                    {
                        if (!File.Exists(".minecraft\\lib\\" + System.IO.Path.GetFileName(libfile)))
                        {
                            File.Copy(libfile, ".minecraft\\lib\\" + System.IO.Path.GetFileName(libfile));
                        }
                    }
                }
                OnImportProgressChangeEvent("处理mods");
                if (Directory.Exists(importFrom + "\\mods"))
                    util.F_H.dircopy(importFrom + "\\mods", ".minecraft\\versions\\" + importName + "\\mods");
                else
                    Directory.CreateDirectory(".minecraft\\versions\\" + importName + "\\mods");
                if (Directory.Exists(importFrom + "\\coremods"))
                    util.F_H.dircopy(importFrom + "\\coremods",
                        ".minecraft\\versions\\" + importName + "\\coremods");
                else
                    Directory.CreateDirectory(".minecraft\\versions\\" + importName + "\\coremods");
                if (Directory.Exists(importFrom + "\\config"))
                    util.F_H.dircopy(importFrom + "\\config", ".minecraft\\versions\\" + importName + "\\config");
                else
                    Directory.CreateDirectory(".minecraft\\versions\\" + importName + "\\configmods");
                OnImportFinish();
                if (callback != null)
                {
                    //BmclCore.Invoke(callback);
                }
            });
            thread.Start();
        }
    }
}
