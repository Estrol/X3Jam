using System;
using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Server.Utils;

namespace Estrol.X3Jam.Server.Handlers {
    public class Channel : Base {
        public Channel(Connection state, PacketManager PM, ServerMain _b) : base(state) {
            Write((short)0x00); // len
            Write(new byte[] {
                0xEB, 0x03, 0x2C, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00
            });

            for (int i = 0; i < 20; i++) {
                ChannelItem ch = _b.ChannelMG.GetChannelByID(i + 1);

                if (ch != null) {
                    Write(new byte[] {
                        0x01, // Wth?
                        0x00,
                        0x00,
                        (byte)(ch.m_ChannelID - 1), // ChannelID
                        (byte)ch.m_MaxRoom, // Channel Max Rooms: 120
                        0x2e, // Channel current rooms;
                        0x00,
                        0x00,
                        0x00,
                        0x00,
                        0x00,
                        0x00,
                        0x00
                    });
                } else {
                    Write(new byte[13]);
                }
            }

            Write(Properties.Resources.Channel);
            //Write(Properties.Resources.EmuChannelListData);

            SetLength((short)m_MemoryStream.Length);
            Console.WriteLine("[Server] [{0}] Get Channel Info!", state.UserInfo.GetUsername());
            Send();
        }
    }
}
