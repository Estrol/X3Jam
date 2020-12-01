using System;
using System.Collections.Generic;
using Estrol.X3Jam.Server.Data;

namespace Estrol.X3Jam.Server.Utils {
    public class ChannelManager {
        public ServerMain main;
        public ChannelItem[] channels;
        public int ChannelCount;

        public ChannelManager(ServerMain main) {
            this.main = main;

            List<ChannelItem> itrs = new List<ChannelItem>();
            int CHCount = main.Config.GetChannelCount();
            ChannelCount = CHCount;

            for (int i = 0; i < CHCount; i++) {
                string data = main.Config.GetChannelByID(i);
                string[] split_data = data.Split(new[] { ',' });

                try {
                    ChannelItem ch = new ChannelItem(i + 1, split_data[0], int.Parse(split_data[1]));
                    itrs.Add(ch);
                } catch (Exception e) {
                    Console.WriteLine("[Server] Failed to load Channel {0}: {1}", i + 1, e.Message);
                    break;
                }
            }

            channels = itrs.ToArray();

            Console.WriteLine("[Server] Loaded like {0} channels", CHCount);
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
