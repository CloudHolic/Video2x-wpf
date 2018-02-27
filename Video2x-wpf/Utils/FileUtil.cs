using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Video2x_wpf.Utils
{
    public static class FileUtil
    {
        public static List<string> GetPngFiles(string directory)
        {
            var fileEntries = new List<string>();
            if (Directory.Exists(directory))
                fileEntries = Directory.GetFiles(directory, "*.png").ToList();

            return fileEntries;
        }
    }
}
