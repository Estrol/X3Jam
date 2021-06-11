using System.Runtime.InteropServices;
using System.Text;

namespace Estrol.X3Jam.Launcher {
    internal class IConfiguration {
        public string path;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public IConfiguration(string INIPath) {
            path = INIPath;
        }

        public void IniWriteValue(string Section, string Key, string Value) {
            WritePrivateProfileString(Section, Key, Value, this.path);
        }

        public string IniReadValue(string Section, string Key) {
            StringBuilder temp = new(255);
            _ = GetPrivateProfileString(Section, Key, "", temp, 255, this.path);
            return temp.ToString();
        }
    }
}
