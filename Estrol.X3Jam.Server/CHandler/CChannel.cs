using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CChannel: CBase {
        public CChannel(Client client) : base(client) {}

        public override void Code() {
            Write((short)0x00); 
            Write(new byte[] {
                0xEB, 0x03, 0x2C, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00
            });

            for (int i = 0; i < 20; i++) {
                Channel ch = m_client.Main.ChannelManager.GetChannelByID(i + 1);

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
            SetL();

            Log.Write("[{0}@{1}] Get channel list.", m_client.UserInfo.Username, m_client.IPAddr);
            Send();
        }
    }
}
