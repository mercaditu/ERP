using System;
using System.IO;

namespace Cognitivo.Class
{
    public static class ErrorLog
    {

        public static void DebeHaber(string Data)
        {
            CreateFile(entity.App.Names.DebeHaberSync, Data, "txt");
        }

        private static void CreateFile(entity.App.Names AppName, string Data, string FileType)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\CogntivoERP";

            //If path (CognitivoERP) does not exist, create path.
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string SubFolder = "\\Logs";

            //If path (TemplateFiles) does not exist, create path
            if (!Directory.Exists(path + SubFolder))
            {
                Directory.CreateDirectory(path + SubFolder);
            }

            path = path + SubFolder + "\\" + AppName + " : " + DateTime.Now.ToString() + "." + FileType;

            using (FileStream fs = File.Create(path))
            {
                using (var fw = new StreamWriter(fs))
                {
                    fw.Write(Data);
                    fw.Flush();
                }
            }
        }
    }
}
