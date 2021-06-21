using System;
using System.IO;

namespace Estrol.X3Jam.Utility {
    public class Configuration {
        public ConfLoader ini;

        public Configuration() { 
            var ConfFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf");
            if (!Directory.Exists(ConfFolder)) {
                Directory.CreateDirectory(ConfFolder);
            }

            var MusicListFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "musiclist");
            if (!Directory.Exists(MusicListFolder)) {
                Directory.CreateDirectory(MusicListFolder);
            }

            var NewsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "News.txt");
            if (!File.Exists(NewsFile)) {
                File.WriteAllText(NewsFile, Properties.Resources.News);
            }

            var ConfFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "server.conf");
            if (!File.Exists(ConfFile)) {
                File.WriteAllBytes(ConfFile, Properties.Resources.Config);
            }

            ini = new ConfLoader(ConfFile);
            Log.Write("Server Configuration Loaded!");
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
                    if (val == null) break;
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
