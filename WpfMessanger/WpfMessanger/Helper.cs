using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace Helper
{
    public static class HelperClass
    {
        public static string CreateFolder(string path)
        {
            string folderPath = "C:\\Users\\elial\\source\\repos\\MessangerC-\\Messanger\\" + path;
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Console.WriteLine("Ordner erstellt: " + folderPath);
            }
            else
            {
                Console.WriteLine("Ordner existiert bereits: " + folderPath);
            }
            return folderPath;
        }
        public static string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }
        public static string FindPathUserId(string userId)
        {
            string basePath = "C:\\Users\\elial\\source\\repos\\MessangerC-\\Messanger\\ClientsDB\\";
            string[] directories = Directory.GetDirectories(basePath);
            foreach (string dir in directories)
            {
                if (dir.EndsWith(userId))
                {
                    return dir;
                }
            }
            return null;
        }
    }
}
