using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CDisconnect: CBase {
        public CDisconnect(Client client) : base(client) { }

        public override void Code() {
            string usr = "null";

            if (Client.UserInfo != null) {
                Channel ch = Client.Main.ChannelManager.GetChannelByID(Client.UserInfo.ChannelID);
                if (ch != null) {
                    ch.RemoveUser(Client.UserInfo);
                }
                usr = Client.UserInfo.Username;
            }

            Log.Write("[{0}@{1}] Disconnected", usr, Client.IPAddr);
            Client.m_socket.Disconnect(true);
        }
    }
}
