using System;
using System.IO;
using Estrol.X3Jam.Server.Utils;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server {
    public class Configuration {
        public INILoader ini;

        public Configuration() {
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
            Log.Write("::Config -> Loaded!");
        }

        public string Get(string section) {
            return ini.IniReadValue("CONFIG", section);
        }

        public void Set(string section, string value) {
            ini.IniWriteValue("CONFIG", section, value);
        }

        public int ChannelCount {
            get {
                int CHCount = 0;

                for (int i = 0; i < 20; i++) {
                    string val = ini.IniReadValue("CHANNELS", string.Format("CH{0}", i + 1));
                    if (val == string.Empty) break;
                    CHCount++;
                }

                return CHCount;
            }
        }

        public string GetChannelByID(int id) {
            return ini.IniReadValue("CHANNELS", string.Format("CH{0}", id + 1));
        }
    }
}
