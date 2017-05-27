using liblauncher.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Forms;
using liblauncher.libs;
using liblauncher.Login;
using System.Globalization;
using ICSharpCode.SharpZipLib.Zip;

namespace liblauncher.launcher
{
    public class lcher
    {

        private readonly Process _game = new Process();
        private readonly string _javaxmx;
        private readonly string _username;
        private readonly string _version;
        private readonly string _name;
        private readonly gameinfo _info;
        private readonly long _timestamp = DateTime.Now.Ticks;
        private readonly string _urlLib = "https://libraries.minecraft.net/";
        public int Downloading = 0;
        private readonly WebClient _downer = new WebClient();
        private LoginInfo _li;
        public string[] Extarg;
        private DateTime _gameStartTime;

        public lcher(string javaPath, string javaXmx, string userName, string name, gameinfo info, string[] extarg, LoginInfo li)
        {
            if (!File.Exists(javaPath))
            {
                throw new NJE();
            }
            _javaxmx = javaXmx;
            _username = userName;
            _version = info.id;
            this._name = name;
            _game.StartInfo.FileName = javaPath;
            _info = info;
            this._li = li;
            this.Extarg = extarg;
            this._info = info;
        }

        #region Events
        public delegate void OnArgsCollectionFinshed(string args);
        public delegate void OnLoadStausChanged(string staus);
        public delegate void OnGameStartup(bool isstarted);
        public delegate void UpdateStausInfo(string info);
        public event OnArgsCollectionFinshed OACF;
        public event OnLoadStausChanged OLSC;
        public event UpdateStausInfo USI;
        public event OnGameStartup OGS;
        #endregion

        public void Start()
        {
            var thread = new Thread(Run);
            thread.Start();
        }

        private void Run()
        {
            _game.StartInfo.UseShellExecute = false;
            OLSC("初始化启动参数....");
            var arg = new StringBuilder("-Xincgc -Xmx");
            arg.Append(_javaxmx);
            arg.Append("M ");
            arg.Append(_solveArgs(Extarg));
            arg.Append(" ");
            arg.Append("-Djava.library.path=\"");
            arg.Append(Core.Config.MCP).Append(@"\.minecraft\versions\");
            arg.Append(_name).Append("\\").Append(_version).Append("-natives-").Append(_timestamp.ToString(CultureInfo.InvariantCulture));
            arg.Append("\" -cp \"");
            OLSC("设置Minecraft启动环境....");
            foreach (libraryies lib in _info.libraries)
            {
                if (lib.natives != null)
                {
                    continue;
                }
                if (lib.rules != null)
                {
                    bool goflag = false;
                    foreach (rules rule in lib.rules)
                    {
                        if (rule.action == "disallow")
                        {
                            if (rule.os == null)
                            {
                                goflag = false;
                                break;
                            }
                            if (rule.os.name.ToLower().Trim() == "windows")
                            {
                                goflag = false;
                                break;
                            }
                        }
                        {
                            if (rule.os == null)
                            {
                                goflag = true;
                                break;
                            }
                            if (rule.os.name.ToLower().Trim() == "windows")
                            {
                                goflag = true;
                                break;
                            }
                        }
                    }
                    if (!goflag)
                    {
                        continue;
                    }
                }
                OLSC("处理Minecraft依赖文件，这可能需要几分钟时间……");
                string libp = BuildLibPath(lib);
                if (GetFileLength(libp) == 0)
                {
                    try
                    {
                        if (lib.url == null)
                        {
                            Downloading++;
                            if (!Directory.Exists(Path.GetDirectoryName(libp)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(libp));
                            }
                            USI("正在获取"+_urlLib + libp.Remove(0, Core.Config.MCP.Length + 22).Replace("\\", "/"));
                            _downer.DownloadFile(
                                _urlLib +
                                libp.Remove(0, Core.Config.MCP.Length + 22).Replace("/", "\\"), libp);

                        }
                        else
                        {
                            string urlLib = lib.url;
                            Downloading++;
                            /*
                            DownLib downer = new DownLib(lib);
                            downLibEvent(lib);
                            downer.DownFinEvent += downfin;
                            downer.startdownload();
                             */
                            if (!Directory.Exists(Path.GetDirectoryName(libp)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(libp));
                            }
                            _downer.DownloadFile(urlLib + libp.Remove(0, Core.Config.MCP.Length + 22).Replace("\\", "/"), libp);
                        }
                    }
                    catch (WebException ex)
                    {
                        try
                        {
                            _downer.DownloadFile("http://bmclapi2.bangbang93.com/libraries/" + libp.Remove(0, Core.Config.MCP.Length + 22).Replace("\\", "/"), libp);
                        }
                        catch (WebException exception)
                        {
                            return;
                        }
                    }
                }
                arg.Append(BuildLibPath(lib) + ";");
            }
            OLSC("应用Minecraft启动参数");
            var mcpath = new StringBuilder(Core.Config.MCP + @"\.minecraft\versions\");
            if (_info.jar == "" || _info.jar == null)
                mcpath.Append(_name).Append("\\").Append(_version).Append(".jar\" ");
            else
                mcpath.Append(_info.jar).Append("\\").Append(_info.jar).Append(".jar\" ");
            mcpath.Append(_info.mainClass);
            arg.Append(mcpath);
            //" --username ${auth_player_name} --session ${auth_session} --version ${version_name} --gameDir ${game_directory} --assetsDir ${game_assets}"
            var mcarg = new StringBuilder(_info.minecraftArguments);
            mcarg.Replace("${auth_player_name}", _username);
            mcarg.Replace("${version_name}", _version);
            mcarg.Replace("${game_directory}", @".");
            mcarg.Replace("${game_assets}", @"assets");
            mcarg.Replace("${assets_root}", @"assets");
            mcarg.Replace("${user_type}", "Legacy");
            mcarg.Replace("${assets_index_name}", _info.assets);
            if (!string.IsNullOrEmpty(_li.OutInfo))
            {
                string[] replace = _li.OutInfo.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in replace)
                {
                    int sp = str.IndexOf(":", System.StringComparison.Ordinal);
                    mcarg.Replace(str.Substring(0, sp), str.Substring(sp + 1));
                    mcarg = new StringBuilder(Microsoft.VisualBasic.Strings.Replace(mcarg.ToString(), str.Split(':')[0], str.Split(':')[1], 1, -1, Microsoft.VisualBasic.CompareMethod.Text));
                }
            }
            else
            {
                mcarg.Replace("{auth_session}", _li.SID);
            }
            mcarg.Replace("${auth_uuid}", Core.Config.GUID);
            mcarg.Replace("${auth_access_token}", Core.Config.GUID);
            mcarg.Replace("${user_properties}", "{}");
            arg.Append(" ");
            arg.Append(mcarg);
            if (Core.exp_lcher)
            {
                OLSC("启动脚本导出完成！");
                OACF(arg.ToString());
                Core.exp_lcher = false;
                return;
            }
            else
            {
                _game.StartInfo.Arguments = arg.ToString();
            }
            Core.exp_lcher = false;
            OLSC("创建依赖项...");
            var nativePath = new StringBuilder(Core.Config.MCP + @"\.minecraft\versions\");
            nativePath.Append(_name).Append("\\");
            var oldnative = new DirectoryInfo(nativePath.ToString());
            foreach (DirectoryInfo dir in oldnative.GetDirectories())
            {
                if (dir.FullName.Contains("-natives-"))
                {
                    Directory.Delete(dir.FullName, true);
                }
            }
            nativePath.Append(_version).Append("-natives-").Append(_timestamp);
            if (!Directory.Exists(nativePath.ToString()))
            {
                Directory.CreateDirectory(nativePath.ToString());
            }
            foreach (libs.libraryies lib in _info.libraries)
            {
                if (lib.natives == null)
                    continue;
                if (lib.rules != null)
                {
                    bool goflag = false;
                    foreach (rules rule in lib.rules)
                    {
                        if (rule.action == "disallow")
                        {
                            if (rule.os == null)
                            {
                                goflag = false;
                                break;
                            }
                            if (rule.os.name.ToLower().Trim() == "windows")
                            {
                                goflag = false;
                                break;
                            }
                        }
                        {
                            if (rule.os == null)
                            {
                                goflag = true;
                                break;
                            }
                            if (rule.os.name.ToLower().Trim() == "windows")
                            {
                                goflag = true;
                                break;
                            }
                        }
                    }
                    if (!goflag)
                    {
                        continue;
                    }
                }
                OLSC("读取依赖文件");
                string[] split = lib.name.Split(':');//0 包;1 名字；2 版本
                if (split.Count() != 3)
                {
                    //throw new UnSupportVersionException();
                }
                string libp = BuildNativePath(lib);
                if (GetFileLength(libp) == 0)
                {
                    {
                        if (lib.url == null)
                        {
                            try
                            {
                                string nativep = BuildNativePath(lib);
                                if (!Directory.Exists(Path.GetDirectoryName(nativep)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(nativep));
                                }
                                _downer.DownloadFile(_urlLib + nativep.Remove(0, Core.Config.MCP.Length + 22).Replace("\\", "/"), nativep);
                            }
                            catch (WebException ex)
                            {
                                string nativep = BuildLibPath(lib);
                                try
                                {
                                    _downer.DownloadFile(
                                        nativep.Remove(0, Core.Config.MCP.Length + 22).Replace("/", "\\"),
                                        nativep);
                                }
                                catch (WebException exception)
                                {
                                    return;
                                }

                            }
                        }
                        else
                        {
                            try
                            {
                                string urlLib = lib.url;
                                /*
                                DownNative downer = new DownNative(lib);
                                downNativeEvent(lib);
                                downer.startdownload();
                                 */
                                string nativep = BuildNativePath(lib);
                                if (!Directory.Exists(Path.GetDirectoryName(nativep)))
                                {
                                    Directory.CreateDirectory(Path.GetDirectoryName(nativep));
                                }
                                _downer.DownloadFile(urlLib + nativep.Replace("/", "\\"), nativep);
                            }
                            catch (WebException ex)
                            {
                                string nativep = BuildLibPath(lib);
                                _downer.DownloadFile("http://bmclapi2.bangbang93.com/libraries/" + nativep.Replace("/", "\\"), nativep);
                            }
                        }
                    }
                }
                var zipfile = new ZipInputStream(System.IO.File.OpenRead(libp));
                ZipEntry theEntry;
                while ((theEntry = zipfile.GetNextEntry()) != null)
                {
                    bool exc = false;
                    OLSC("加载" + theEntry.Name);
                    if (lib.extract.exclude != null)
                    {
                        if (lib.extract.exclude.Any(excfile => theEntry.Name.Contains(excfile)))
                        {
                            exc = true;
                        }
                    }
                    if (exc) continue;
                    var filepath = new StringBuilder(nativePath.ToString());
                    filepath.Append("\\").Append(theEntry.Name);
                    FileStream fileWriter = File.Create(filepath.ToString());
                    var data = new byte[2048];
                    while (true)
                    {
                        int size = zipfile.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            fileWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                    fileWriter.Close();

                }
            }
            if (Directory.Exists(@".minecraft\versions\" + _name + @"\mods"))
            {
                if (Directory.Exists(@".minecraft\Config"))
                {
                    Directory.Move(@".minecraft\Config", @".minecraft\Config" + _timestamp);
                    if (Directory.Exists(@".minecraft\versions\" + _name + @"\Config"))
                        F_H.dircopy(@".minecraft\versions\" + _name + @"\Config", @".minecraft\Config");
                }
                else
                {
                    if (Directory.Exists(@".minecraft\versions\" + _name + @"\Config"))
                        F_H.dircopy(@".minecraft\versions\" + _name + @"\Config", @".minecraft\Config");
                }
                if (Directory.Exists(@".minecraft\mods"))
                {
                    Directory.Move(@".minecraft\mods", @".minecraft\mods" + _timestamp);
                    if (Directory.Exists(@".minecraft\versions\" + _name + @"\mods"))
                        F_H.dircopy(@".minecraft\versions\" + _name + @"\mods", @".minecraft\mods");
                }
                else
                {
                    if (Directory.Exists(@".minecraft\versions\" + _name + @"\mods"))
                        F_H.dircopy(@".minecraft\versions\" + _name + @"\mods", @".minecraft\mods");
                }
                if (Directory.Exists(@".minecraft\coremods"))
                {
                    Directory.Move(@".minecraft\coremods", @".minecraft\coremods" + _timestamp);
                    if (Directory.Exists(@".minecraft\versions\" + _name + @"\coremods"))
                        F_H.dircopy(@".minecraft\versions\" + _name + @"\coremods", @".minecraft\coremods");
                }
                else
                {
                    if (Directory.Exists(@".minecraft\versions\" + _name + @"\coremods"))
                        F_H.dircopy(@".minecraft\versions\" + _name + @"\coremods", @".minecraft\coremods");
                }
                if (Directory.Exists(@".minecraft\versions\" + _name + @"\moddir"))
                {
                    var moddirs = new DirectoryInfo(@".minecraft\versions\" + _name + @"\moddir");
                    foreach (DirectoryInfo moddir in moddirs.GetDirectories())
                    {
                        F_H.dircopy(moddir.FullName, ".minecraft\\" + moddir.Name);
                    }
                    foreach (FileInfo modfile in moddirs.GetFiles())
                    {
                        File.Copy(modfile.FullName, ".minecraft\\" + modfile.Name, true);
                    }
                }
            }

            Environment.SetEnvironmentVariable("APPDATA", Core.Config.MCP);
            _game.EnableRaisingEvents = true;
            _game.StartInfo.WorkingDirectory = Core.Config.MCP + "\\.minecraft";
            try
            {
                bool fin = _game.Start();
                _gameStartTime = new DateTime();
                OGS(true);
            }
            catch
            {
                OGS(false);
            }
        }
        /// <summary>
        /// 获取lib文件的绝对路径
        /// </summary>
        /// <param name="lib"></param>
        /// <returns></returns>
        public static string BuildLibPath(libraryies lib)
        {
            var libp = new StringBuilder(Core.Config.MCP + @"\.minecraft\libraries\");
            string[] split = lib.name.Split(':');//0 包;1 名字；2 版本
            if (split.Count() != 3)
            {
                //throw new UnSupportVersionException();
            }
            libp.Append(split[0].Replace('.', '\\'));
            libp.Append("\\");
            libp.Append(split[1]).Append("\\");
            libp.Append(split[2]).Append("\\");
            libp.Append(split[1]).Append("-");
            libp.Append(split[2]).Append(".jar");
            return libp.ToString();
        }

        /// <summary>
        /// 获取native文件的绝对路径
        /// </summary>
        /// <param name="lib"></param>
        /// <returns></returns>
        public static string BuildNativePath(libraryies lib)
        {
            var libp = new StringBuilder(Core.Config.MCP + @"\.minecraft\libraries\");
            string[] split = lib.name.Split(':');//0 包;1 名字；2 版本
            libp.Append(split[0].Replace('.', '\\'));
            libp.Append("\\");
            libp.Append(split[1]).Append("\\");
            libp.Append(split[2]).Append("\\");
            libp.Append(split[1]).Append("-").Append(split[2]).Append("-").Append(lib.natives.windows);
            libp.Append(".jar");
            if (split[0] == "tv.twitch")
            {
                libp.Replace("${arch}", Environment.Is64BitOperatingSystem ? "64" : "32");
            }
            return libp.ToString();
        }

        public bool IsRunning()
        {
            return !_game.HasExited;
        }

        /// <summary>
        /// GetFileLength
        /// </summary>
        /// <param name="path"></param>
        /// <returns>FileLength,if file doesn't exist return 0</returns>
        private long GetFileLength(string path)
        {
            try
            {
                return (new FileInfo(path)).Length;
            }
            catch (IOException)
            {
                return 0;
            }
        }

        private string _solveArgs(string[] args)
        {
            var arg = new StringBuilder();
            for (int i = 0; i < args.Length; i++)
            {
                if (i == 0 || args[i][0] == '"')
                {
                    arg.Append(args[i]).Append(' ');
                }
                else
                {
                    arg.Append('"').Append(args[i]).Append("\" ");
                }
            }
            return arg.ToString();
        }

        public int GetUpTime()
        {
            return (int)(new DateTime() - _gameStartTime).TotalSeconds;
        }
    }
}
