﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace liblauncher.util
{
    public static class F_H
    {
        static public void dircopy(string from, string to)
        {
            DirectoryInfo dir = new DirectoryInfo(from);
            if (!Directory.Exists(to))
            {
                Directory.CreateDirectory(to);
            }
            foreach (DirectoryInfo sondir in dir.GetDirectories())
            {
                dircopy(sondir.FullName, to + "\\" + sondir.Name);
            }
            foreach (FileInfo file in dir.GetFiles())
            {
                File.Copy(file.FullName, to + "\\" + file.Name, true);
            }
        }

        static public bool IfFileVaild(string Path, long Length = -1)
        {
            if (!File.Exists(Path))
            {
                return false;
            }
            if (new FileInfo(Path).Length == 0)
            {
                return false;
            }
            if (Length != -1)
            {
                if (new FileInfo(Path).Length != Length)
                    return false;
            }
            return true;
        }

        static public void CreateDirectoryIfNotExist(string Dir)
        {
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
        }

        static public void CreateDirectoryForFile(string File)
        {
            CreateDirectoryIfNotExist(Path.GetDirectoryName(File));
        }

        public static void WriteFile(string path, string content)
        {
            var directory = System.IO.Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            File.WriteAllText(path, content);
        }
    }
}
