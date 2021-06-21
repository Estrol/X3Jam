using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CChannel: CBase {
        public CChannel(Client client) : base(client) {}

        public override void Code() {
            Write((short)0x00); 
            Write(new byte[] {
                0xEB, 0x03, 0x2C, 0x01, 0x00, 0x00, 0x00, 0x00
            });

            for (int i = 0; i < 40; i++) {
                Channel ch = Main.ChannelManager.GetChannelByID(i + 1);

                if (ch != null) {
                    Write((short)(ch.m_ChannelID - 1));
                    Write((short)ch.m_MaxRoom);
                    Write((short)0);
                    Write((short)ch.Count); // Current users
                    Write((short)0);
                    Write((short)1); // idk
                    Write((byte)0); // Padding?
                } else {
                    Write(new byte[13]);
                }
            }

            SetL();

            Log.Write("[{0}@{1}] Get channel list.", Client.UserInfo.Username, Client.IPAddr);
            Send();
        }
    }
}
