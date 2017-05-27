using liblauncher;
using liblauncher.launcher;
using liblauncher.Login;
using MCLauncher.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MCLauncher
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        static string[] names_ = new string[] { "genral", "res", "mod_mana", "abt","setting" };
        ArrayList names = new ArrayList(names_);
        public MainWindow()
        {
            InitializeComponent();
            Core ce = new Core(AppDomain.CurrentDomain.BaseDirectory);
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "//" + "bgk.skn"))
            {
                ImageBrush b = new ImageBrush();
                b.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "//" + "bgk.skn", UriKind.Absolute));
                Background = b;
            }
            var da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(2));
            g.BeginAnimation(OpacityProperty, da);
            //this.BeginAnimation(OpacityProperty, da);
            CC.Content = new gen();
            
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        int linx = 0;
        private void Click(object sender, RoutedEventArgs e)
        {
            int inx = 0;
            Button b = (Button)sender;
            var da = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            CC.BeginAnimation(OpacityProperty, da);
            inx = names.IndexOf(b.Name);
            Button b_tmp,b_tmp2;
            if (b.Name.Equals("res"))
            {
                b_tmp = (Button)FindName(names_[linx]);
                b_tmp.Background = null;
                CC.Content = new RESOURCES();
                b_tmp2 = FindName(names_[inx]) as Button;
                b_tmp2.Background = (new BrushConverter()).ConvertFromInvariantString("#4C40FF00") as Brush;
                linx = inx;
            }
            else if(b.Name.Equals("mod_mana"))
            {
                b_tmp = (Button)FindName(names_[linx]);
                b_tmp.Background = null;
                CC.Content = new Mod();
                b_tmp2 = FindName(names_[inx]) as Button;
                b_tmp2.Background = (new BrushConverter()).ConvertFromInvariantString("#4C40FF00") as Brush;
                linx = inx;
            }
            else if (b.Name.Equals("abt"))
            {
                b_tmp = (Button)FindName(names_[linx]);
                b_tmp.Background = null;
                CC.Content = new abt();
                b_tmp2 = FindName(names_[inx]) as Button;
                b_tmp2.Background = (new BrushConverter()).ConvertFromInvariantString("#4C40FF00") as Brush;
                linx = inx;
            }
            else if (b.Name.Equals("genral"))
            {
                b_tmp = FindName(names_[linx]) as Button;
                b_tmp.Background = null;
                CC.Content = new gen();
                b_tmp2 = FindName(names_[inx]) as Button;
                b_tmp2.Background = (new BrushConverter()).ConvertFromInvariantString("#4C40FF00") as Brush;
                linx = inx;
            }
            else if (b.Name.Equals("setting"))
            {
                b_tmp = FindName(names_[linx]) as Button;
                b_tmp.Background = null;
                CC.Content = new setting();
                b_tmp2 = FindName(names_[inx]) as Button;
                b_tmp2.Background = (new BrushConverter()).ConvertFromInvariantString("#4C40FF00") as Brush;
                linx = inx;
            }
            da = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1));
            CC.BeginAnimation(OpacityProperty, da);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var da = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(2));
            g.BeginAnimation(OpacityProperty, da);
            this.BeginAnimation(OpacityProperty, da);
        }

        private void FakeLogin(object sender, RoutedEventArgs e)
        {
            gen g = CC.Content as gen;
            if (string.IsNullOrEmpty(g.mcp.Text) || string.IsNullOrEmpty(g.javap.Text) || string.IsNullOrEmpty(g.jmen.Text) || string.IsNullOrEmpty(gen.mcver))
            {
                MessageBox.Show("请填写相应的参数(Minecraft路径,Java路径以及内存大小)","错误",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
            LoginInfo LI = new LoginInfo();
            LI.SID = Guid.NewGuid().ToString();
            LI.UN = g.uname.Text;
            LI.Suc = true;
            string java = Core.Config.Javaw;
            string javaxm = Core.Config.Javaxmx;
            string arg = Core.Config.ExtraJvmArg;
            lcher Game = new lcher(java, javaxm, Core.Config.Username, gen.mcver, Core.gi, new[] { arg }, LI);
            Game.OGS += Game_OGS;
            Game.OLSC += Game_OLSC;
            Game.USI += Game_USI;
            run(Game);
        }

        private void Game_USI(string info)
        {
            SInfo.Dispatcher.Invoke(new Action(() =>
            {
                SInfo.Content = info;
            }));
        }

        private void purchased(object sender, RoutedEventArgs e)
        {
            gen g = CC.Content as gen;
            yggdrasil.NewLogin nl = new yggdrasil.NewLogin();
            yggdrasil.NewLogin.LoginInfo li = nl.Login(g.uname.Text, g.pwd.Text);
            LoginInfo LI = new LoginInfo();
            LI.SID = li.SID;
            LI.UN = li.UN;
            LI.Suc = li.Suc;
            LI.Client_identifier = li.Client_identifier;
            LI.Errinfo = li.Errinfo;
            LI.UID = li.UID;
            LI.OutInfo = li.OtherInfo;
            LI.OtherInfo = li.OtherInfo;
            string java = Core.Config.Javaw;
            string javaxm = Core.Config.Javaxmx;
            string arg = Core.Config.ExtraJvmArg;
            lcher Game = new lcher(java, javaxm, Core.Config.Username, gen.mcver, Core.gi, new[] { arg }, LI);
            Game.OGS += Game_OGS;
            Game.OLSC += Game_OLSC; ;
            run(Game);
        }

        public static void run(lcher g)
        {
            if (Core.exp_lcher)
            {
                g.OACF += Game_OACF;
            }
            else
            {
                g.Start();
            }
        }

        private void Game_OGS(bool isstarted)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                Application.Current.Shutdown();
            }
            ));
            
        }
        
        private void Game_OLSC(string staus)
        {
            stau_lable.Dispatcher.Invoke(new Action(() =>
            {
                stau_lable.Content = staus;
            }
            ));
        }

        private static void Game_OACF(string args)
        {
            FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\" + gen.mcver + ".bat",FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            sw.Write(Core.Config.Javaw+args);
            
        }
    }
}
