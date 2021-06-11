using System;
using System.Collections.Generic;
using System.IO;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CManager {
    public class ChanManager {
        public O2JamServer main;
        public Channel[] channels;
        public int ChannelCount;

        public ChanManager(O2JamServer main) {
            this.main = main;

            List<Channel> itrs = new List<Channel>();
            int CHCount = main.Config.ChannelCount;
            ChannelCount = 0;

            bool isError = false;
            int CHItr = 0;
            string listErrorName = "";

            for (int i = 0; i < CHCount; i++) {
                string data = main.Config.GetChannelByID(i);
                string[] split_data = data.Split(new[] { ',' });

                try {
                    Channel ch = new(main, this, i + 1, split_data[0], int.Parse(split_data[1]));
                    itrs.Add(ch);

                    ChannelCount++;
                } catch (Exception e) {
                    if (e is FileNotFoundException) {
                        string msg = e.Message.Replace(AppDomain.CurrentDomain.BaseDirectory, Path.DirectorySeparatorChar.ToString());

                        Log.Write("Failed to load channel {0}: {1}", i + 1, msg);
                    } else {
                        Log.Write("Failed to load channel {0}: {1}", i + 1, e.Message);
                    }

                    CHItr = i + 1;
                    listErrorName = split_data[0];
                    isError = true;
                    break;
                }
            }

            channels = itrs.ToArray();
            if (isError) {
                Log.Write("One of channels failed to load try checking if {0} is defined in CH{1}!", listErrorName, CHItr);
                Environment.Exit(-1);
            }

            Log.Write("{0} Channels Loaded!", ChannelCount);
        }

        public Channel GetChannelByID(int ID) {
            if (ID < 0) return null;

            for (int i = 0; i < channels.Length; i++) {
                if (channels[i].m_ChannelID == ID) return channels[i];
            }

            return null;
        }
    }
}
