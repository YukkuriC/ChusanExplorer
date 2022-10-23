using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ChusanExplorer
{
    public static class Config
    {
        public const string ALL = "(All)";

        public static string DirA000 = "C:/Chunithm New Plus/app/data/A000";
        public static string DirOptions = "C:/Chunithm New Plus/option";
        public static string DirAquaDB = "C:/Chunithm New Plus/aqua-0.0.31-RELEASE/data/db.sqlite";

        public static bool MuteErrors = false;

        static ReadWriteINIfile ini;
        static string iniPath = Path.GetFullPath("config.ini");
        const string grpData = "Game Files",
                     grpDB = "Aqua Files",
                     grpSettings = "Settings";
        const string keyA000 = "path_to_A000",
                     keyOption = "path_to_option",
                     keyDB = "path_to_db",
                     keyMuteError = "mute_errors";

        public static void Init()
        {
            ini = new ReadWriteINIfile(iniPath);
            if (File.Exists(iniPath))
            {
                // read
                DirA000 = Path.GetFullPath(ini.ReadINI(grpData, keyA000));
                DirOptions = Path.GetFullPath(ini.ReadINI(grpData, keyOption));
                DirAquaDB = Path.GetFullPath(ini.ReadINI(grpDB, keyDB));
                bool.TryParse(ini.ReadINI(grpSettings, keyMuteError), out MuteErrors);
            }
            else
            {
                // write
                ini.WriteINI(grpData, keyA000, DirA000);
                ini.WriteINI(grpData, keyOption, DirOptions);
                ini.WriteINI(grpDB, keyDB, DirAquaDB);
                ini.WriteINI(grpSettings, keyMuteError, MuteErrors.ToString());
            }
        }
    }

    public class ReadWriteINIfile
    {
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string name, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public string path;

        public ReadWriteINIfile(string inipath)
        {
            path = inipath;
        }
        public void WriteINI(string name, string key, string value)
        {
            WritePrivateProfileString(name, key, value, this.path);
        }
        public string ReadINI(string name, string key)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(name, key, "", sb, 255, this.path);
            return sb.ToString();
        }
    }
}
