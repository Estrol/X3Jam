using System;
using System.IO;
using Estrol.X3Jam.Server.Utils;

namespace Estrol.X3Jam.Server {
    public class Config {
        public INILoader ini;

        public Config() {
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\conf")) {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\conf");
            }

            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\conf\musiclist")) {
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\conf\musiclist");
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\conf\News.txt")) {
                File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + @"\conf\News.txt", Properties.Resources.News);
            }

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\conf\server.conf")) {
                File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\conf\server.conf", Properties.Resources.Config);
            }

            ini = new INILoader(AppDomain.CurrentDomain.BaseDirectory + @"\conf\server.conf");
            Console.WriteLine("[Server] Config Loaded!");
        }

        public string GetConfigValue(string section) {
            return ini.IniReadValue("CHANNELS", section);
        }

        public void SetConfigValue(string section, string value) {
            ini.IniWriteValue("CHANNELS", section, value);
        }

        public int GetChannelCount() {
            int CHCount = 0;

            for (int i = 0; i < 20; i++) {
                string val = ini.IniReadValue("CHANNELS", string.Format("CH{0}", i + 1));
                if (val == string.Empty) break;
                CHCount++;
            }

            return CHCount;
        }

        public string GetChannelByID(int id) {
            return ini.IniReadValue("CHANNELS", string.Format("CH{0}", id + 1));
        }
    }
}
