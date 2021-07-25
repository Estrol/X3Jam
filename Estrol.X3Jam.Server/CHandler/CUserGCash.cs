using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CUserGCash: CBase {
        public CUserGCash(Client client) : base(client) { }

        public override void Code() {
            int GCash = Client.UserInfo.Char.Gold;

            Log.Write("[{0}@{1}] Get Player's Money about: {2}",
                Client.UserInfo.Username,
                Client.IPAddr,
                GCash);

            Write((short)8);
            Write((short)0x13a5);
            Write(GCash); // int32 GCash
            Send();
        }
    }
}
