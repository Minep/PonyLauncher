using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCLauncher.zipSupport
{
    public class ZipManager
    {
        public static double total;
        public static double total_f;
        int count = 0;
        public delegate void UnCompressing(string currentFile, double percent);
        public event UnCompressing uncom;
        public delegate void Compressing(string currentFile, double percent);
        public event Compressing com;
        public delegate void Length(double d);
        public event Length l;
        //public ArrayList failName;
        //public ArrayList failName_org;
        /// <summary>
        /// Unzip the file
        /// </summary>
        /// <param name="args">0=>Input Zip File; 1=>Output Directory</param>
        public void UnZip(string[] args)
        {
            count = 0;
            ZipInputStream s = new ZipInputStream(File.OpenRead(args[0]));
            ZipEntry theEntry;
            //failName = new ArrayList();
            //failName_org = new ArrayList();
            total = new FileInfo(args[0]).Length;
            l(total);
            
            int cnt = 0;
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string directoryName = Path.GetDirectoryName(args[1]);
                string fileName = Path.GetFileName(theEntry.Name);
                //生成解压目录
                Directory.CreateDirectory(directoryName);
                if (fileName != String.Empty)
                {
                    
                    //解压文件到指定的目录
                    if (!string.IsNullOrEmpty(Path.GetDirectoryName(theEntry.Name)))
                    {
                        Directory.CreateDirectory(args[1] +"\\"+ Path.GetDirectoryName(theEntry.Name));
                    }
                    FileStream streamWriter = null;
                    try
                    {
                        streamWriter = File.Create(args[1] + "\\" + theEntry.Name);
                        uncom(theEntry.Name, s.Position);
                        int size = 2048;
                        byte[] data = new byte[2048];
                        while (true)
                        {
                            size = s.Read(data, 0, data.Length);
                            if (size > 0)
                            {
                                streamWriter.Write(data, 0, size);
                            }
                            else
                            {
                                break;
                            }
                        }
                        streamWriter.Close();
                    }
                    catch(ArgumentException ae)
                    {
                        //string _fn = Path.GetFileNameWithoutExtension(theEntry.Name);
                        //string _fe = Path.GetExtension(theEntry.Name);
                        //string _fp = Path.GetDirectoryName(theEntry.Name);
                        //streamWriter = File.Create(args[1] + @"\" + _fp+@"\EEEEf"+cnt+_fe);
                        //failName.Add(_fp + @"\EEEEf" + cnt + _fe);
                        //failName_org.Add(theEntry.Name);
                        //cnt++;
                    }
                    //Console.WriteLine(theEntry.Name);
                }
            }
            s.Close();
            //string[] _f = (string[])failName.ToArray(typeof(string));
            //string[] _f_o = (string[])failName_org.ToArray(typeof(string));
            //for(int i=0;i<_f.Length;i++)
            //{
            //    FileInfo fi = new FileInfo(args[1] + _f[i]);
            //    fi.MoveTo(args[1] +@"\"+ _f_o[i]);
            //}
        }

        public void ZipFile(string FileToZip, string ZipedFile, int CompressionLevel, int BlockSize)
        {
            //如果文件没有找到，则报错
            if (!System.IO.File.Exists(FileToZip))
            {
                throw new System.IO.FileNotFoundException("The specified file " + FileToZip + " could not be found. Zipping aborderd");
            }

            System.IO.FileStream StreamToZip = new System.IO.FileStream(FileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.FileStream ZipFile = System.IO.File.Create(ZipedFile);
            ZipOutputStream ZipStream = new ZipOutputStream(ZipFile);
            ZipEntry ZipEntry = new ZipEntry("ZippedFile");
            ZipStream.PutNextEntry(ZipEntry);
            
            ZipStream.SetLevel(CompressionLevel);
            byte[] buffer = new byte[BlockSize];
            System.Int32 size = StreamToZip.Read(buffer, 0, buffer.Length);
            ZipStream.Write(buffer, 0, size);
            try
            {
                while (size < StreamToZip.Length)
                {
                    int sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                    ZipStream.Write(buffer, 0, sizeRead);
                    size += sizeRead;
                }
            }
            catch (System.Exception ex)
            {
                throw ex;
            }
            ZipStream.Finish();
            ZipStream.Close();
            StreamToZip.Close();
        }

        /// <summary>
        /// Zip files
        /// </summary>
        /// <param name="args">0=>Input Directory; 1=>Output file</param>
        public void ZipFileMain(string[] args)
        {
            string[] filenames = Directory.GetFiles(args[0]);
            count = 0;
            Crc32 crc = new Crc32();
            ZipOutputStream s = new ZipOutputStream(File.Create(args[1]));
            total_f = filenames.Length;
            l(total_f);
            s.SetLevel(6); // 0 - store only to 9 - means best compression

            foreach (string file in filenames)
            {
                count++;
                //打开压缩文件
                FileStream fs = File.OpenRead(file);
                com(file, count);
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                ZipEntry entry = new ZipEntry(file);
                entry.DateTime = DateTime.Now;

                // set Size and the crc, because the information
                // about the size and crc should be stored in the header
                // if it is not set it is automatically written in the footer.
                // (in this case size == crc == -1 in the header)
                // Some ZIP programs have problems with zip files that don't store
                // the size and crc in the header.
                entry.Size = fs.Length;
                fs.Close();

                crc.Reset();
                crc.Update(buffer);

                entry.Crc = crc.Value;

                s.PutNextEntry(entry);

                s.Write(buffer, 0, buffer.Length);

            }

            s.Finish();
            s.Close();
        }

        public void Zipping(string source,string destination)
        {
            FastZip fz = new FastZip();
            fz.CreateEmptyDirectories = true;
            fz.CreateZip(destination, source,true,"");
        }

        public void ExtractEssentialFile(string filp,string dest)
        {
            count = 0;
            ZipInputStream s = new ZipInputStream(File.OpenRead(filp));
            ZipEntry theEntry;
            total = new FileInfo(filp).Length;
            l(total);
            while ((theEntry = s.GetNextEntry()) != null)
            {
                string[] str = theEntry.Name.Split('.');
                if (str[str.Length-1].Equals("jar")|| str[str.Length - 1].Equals("json"))
                {
                    FileStream streamWriter = File.Create(dest + "\\" + theEntry.Name);
                    uncom(theEntry.Name, s.Position);
                    int size = 2048;
                    byte[] data = new byte[2048];
                    while (true)
                    {
                        size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }
                    streamWriter.Close();
                }
            }
        }
    }
    
}
