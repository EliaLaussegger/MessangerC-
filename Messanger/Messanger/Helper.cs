using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
