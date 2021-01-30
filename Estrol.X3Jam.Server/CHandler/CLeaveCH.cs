using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public class CLeaveCH: CBase {
        public CLeaveCH(Client client) : base(client) {}

        public override void Code() {
            Channel channel = ChanManager.GetChannelByID(Client.UserInfo.ChannelID);
            channel.RemoveUser(Client.UserInfo);
            Client.UserInfo.ChannelID = -1;

            Write(new byte[] {
                0x08, 00, 0xe6, 0x07, 0x00, 0x00, 0x00, 0x00
            });

            Send();
        }
    }
}
