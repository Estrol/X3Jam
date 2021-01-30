using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CEnterCH: CBase {
        public CEnterCH(Client client) : base(client) { }

        public override void Code() {
            int ChID = Client.Message.data[4] + 1;
            if (ChID > Client.Main.ChannelManager.ChannelCount) {
                Log.Write("[{0}@{1}] Attempting to enter channel: {2}, that doesn't exists in server!",
                    Client.UserInfo.Username,
                    Client.IPAddr,
                    ChID
                );
            } else {
                Log.Write("[{0}@{1}] Entering channel: {2}",
                    Client.UserInfo.Username,
                    Client.IPAddr,
                    ChID
                );

                Client.UserInfo.ChannelID = ChID;
                var CH = Client.Main.ChannelManager.GetChannelByID(ChID);
                CH.AddUser(Client.UserInfo);

                Write(new byte[] {
                    0x10, 0x00, 0xed, 0x03, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 // 0x01, 0x00, 0x00, 0x00 -> Player Ranking in leaderboard
                });

                Send();
            }
        }
    }
}
