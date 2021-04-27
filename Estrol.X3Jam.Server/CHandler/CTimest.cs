using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CTimest: CBase {
        public CTimest(Client client) : base(client) { }

        public override void Code() {
            Log.Write("[{0}@{1}] Get Player's Money about: 5000",
                Client.UserInfo.Username,
                Client.IPAddr);

            Write((short)8);
            Write((short)0x13a5);
            Write(5000); // int32 GCash
            Send();
        }
    }
}
