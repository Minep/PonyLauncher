using liblauncher.launcher;
using System.Globalization;
using System.IO;

namespace liblauncher
{
    public class Core
    {
        public static Config Config;
        public static string cd;
        public static gameinfo gi;
        public static bool exp_lcher = false;
        public static string lcher_args = "";
        public Core(string CurrDirectory)
        {
            cd = CurrDirectory + "setup.cfg";
            if (File.Exists(cd))
            {
                Config = Config.Load(cd);
            }
            else
            {
                Config = new Config();
            }
        }
        public static void set(string BaseDirectory)
        {
            if (!Directory.Exists(BaseDirectory))
            {
                Directory.CreateDirectory(BaseDirectory);
            }
            if (Config.Javaw == "autosearch")
            {
                Config.Javaw = Config.GetJavaDir();
            }
            if (Config.Javaxmx == "autosearch")
            {
                Config.Javaxmx = (Config.GetMemory() / 4).ToString(CultureInfo.InvariantCulture);
            }
        }
        public static void savecfg()
        {
            Config.Save(cd);
        }
    }
}
