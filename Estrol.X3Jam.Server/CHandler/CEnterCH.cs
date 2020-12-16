using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CEnterCH: CBase {
        public CEnterCH(Client client) : base(client) { }

        public override void Code() {
            int ChID = m_client.Message.data[4] + 1;
            if (ChID > m_client.Main.ChannelManager.ChannelCount) {
                Log.Write("[{0}@{1}] Attempting to enter channel: {2}, that doesn't exists in server!",
                    m_client.UserInfo.Username,
                    m_client.IPAddr,
                    ChID
                );
            } else {
                Log.Write("[{0}@{1}] Entering channel: {2}",
                    m_client.UserInfo.Username,
                    m_client.IPAddr,
                    ChID
                );

                m_client.UserInfo.ChannelID = ChID;
                var CH = m_client.Main.ChannelManager.GetChannelByID(ChID);
                CH.AddUser(m_client.UserInfo);

                Write(new byte[] {
                    0x10, 0x00, 0xed, 0x03, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 // 0x01, 0x00, 0x00, 0x00 -> Player Ranking in leaderboard
                });

                Send();
            }
        }
    }
}
