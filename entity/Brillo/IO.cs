using System;
using System.IO;

namespace entity.Brillo
{
    public static class IO
    {
        /// <summary>
        /// This function will create a path if it does not previously exist.
        /// </summary>
        /// <param name="path">Full Path</param>
        /// <returns>Full Path but verified</returns>
        public static string CreateIfNotExists(string path)
        {
            string directory = string.Empty;
            int index = path.LastIndexOf(@"\");
            if (index > 0)
                directory = path.Substring(0, index);

            if (Directory.Exists(directory) == false)
            {
                DirectoryInfo di = Directory.CreateDirectory(directory);
            }

            return path;
        }

        public static bool FileExists(string path)
        {
            return File.Exists(path);
            
        }
      
    }
}