using System;
using System.Collections.Generic;
using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.Utils {
    public class ChannelManager {
        public ServerMain main;
        public ChannelItem[] channels;
        public int ChannelCount;

        public ChannelManager(ServerMain main) {
            this.main = main;

            List<ChannelItem> itrs = new List<ChannelItem>();
            int CHCount = main.Config.GetChannelCount();
            ChannelCount = 0;

            bool isError = false;

            for (int i = 0; i < CHCount; i++) {
                string data = main.Config.GetChannelByID(i);
                string[] split_data = data.Split(new[] { ',' });

                try {
                    ChannelItem ch = new ChannelItem(i + 1, split_data[0], int.Parse(split_data[1]));
                    itrs.Add(ch);

                    ChannelCount++;
                } catch (Exception e) {
                    if (e is System.IO.FileNotFoundException) {
                        string msg = e.Message.Replace(AppDomain.CurrentDomain.BaseDirectory, @"\");

                        Log.Write("Failed to load channel {0}: {1}", i + 1, msg);
                    } else {
                        Log.Write("Failed to load channel {0}: {1}", i + 1, e.Message);
                    }
                    
                    isError = true;
                    break;
                }
            }

            channels = itrs.ToArray();
            if (isError) {
                Log.Write("One of channels failed to load try checking if OJNList.dat is defined!");
            }

            Log.Write("Loaded like {0} channels", ChannelCount);
        }

        public ChannelItem GetChannelByID(int ID) {
            if (ID < 0) return null;

            for (int i = 0; i < channels.Length; i++) {
                if (channels[i].m_ChannelID == ID) return channels[i];
            }

            return null;
        }
    }
}
