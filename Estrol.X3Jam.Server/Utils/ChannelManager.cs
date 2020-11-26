using Estrol.X3Jam.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.Utils {
    public class ChannelManager {
        public ChannelItem[] channels;

        public ChannelManager() {
            channels = new ChannelItem[] { 
                new ChannelItem(1),
                new ChannelItem(2),
                new ChannelItem(3),
                new ChannelItem(4)
            };
        }

        public ChannelItem GetChannelByID(int ID) {
            for (int i = 0; i < channels.Length; i++) {
                if (channels[i].ChannelID == ID) return channels[i];
            }

            return null;
        }
    }
}
