using System;
using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Server.Utils;

namespace Estrol.X3Jam.Server.Handlers {
    public class Channel : Base {
        public Channel(Connection state, PacketManager PM) : base(state) {
            /** -- TODO finish this when Implement multiplayer
            Write(new byte[] {
                0x44, 0x0f, // Packet length
                0xeb, 0x03, // Channel opcode
            });

            Write(new byte[] {
                0x2c, 0x01, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00
            });

            for (int i = 0; i < 20; i++) {
                if (i < 4) {
                    Write(new byte[] {
                        0x00, 0x00, 0x00, 0x00,
                        0x00, 0x00, 0x00, 0x00,
                        0x01, 0x00, 0x00
                    });

                    Write(new byte[] {
                        (byte)(i + 1),
                        0x00, 0x78
                    });
                } else {
                    Write(new byte[14]);
                }
            }

            Write(new byte[7]);
            Write(new byte[] { 0x01 });
            **/
            Write(Properties.Resources.EmuChannelListData);

            Console.WriteLine("[Server] [{0}] Get Channel Info!", state.UserInfo.GetUsername());
            Send();
        }
    }
}
