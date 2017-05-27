using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MCLauncher
{
    class DownloadTask
    {
        public delegate void ODF();
        public event ODF OnDownloadFinish;
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="URL">文件URL</param>
        /// <param name="filename">文件保存路径</param>
        /// <param name="prog"></param>
        /// <param name="label1"></param>
        public void DownloadFile(string URL, string filename,ProgressBar prog,Label label1)
        {

            float percent = 0;
            try
            {
                System.Net.HttpWebRequest Myrq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                System.Net.HttpWebResponse myrp = (System.Net.HttpWebResponse)Myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (prog != null)
                {
                    prog.Dispatcher.Invoke(new Action(() =>
                    {
                        prog.Maximum = (int)totalBytes;
                    }
                    ));
                }
                System.IO.Stream st = myrp.GetResponseStream();
                System.IO.Stream so = new System.IO.FileStream(filename, System.IO.FileMode.Create);
                long totalDownloadedByte = 0;
                byte[] by = new byte[1024];
                int osize = st.Read(by, 0, (int)by.Length);
                while (osize > 0)
                {
                    totalDownloadedByte = osize + totalDownloadedByte;
                    System.Windows.Forms.Application.DoEvents();
                    so.Write(by, 0, osize);
                    if (prog != null)
                    {
                        prog.Dispatcher.Invoke(new Action(() =>
                        {
                            prog.Value = (int)totalDownloadedByte;
                        }
                        ));
                    }
                    osize = st.Read(by, 0, (int)by.Length);

                    percent = (float)Math.Round((double)totalDownloadedByte / (double)totalBytes * 100);
                    label1.Dispatcher.Invoke(new Action(()=>
                    {
                        label1.Content = "已下载："+totalDownloadedByte/1024+"KB/"+totalBytes/1024+"KB     "+percent.ToString()+"%";
                    }
                    ));
                    //System.Windows.Forms.Application.DoEvents(); //必须加注这句代码，否则label1将因为循环执行太快而来不及显示信息
                }
                so.Close();
                st.Close();
                OnDownloadFinish();
            }
            catch (System.Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
