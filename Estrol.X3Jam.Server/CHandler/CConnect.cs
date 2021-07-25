using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CConnect : CBase {
        public CConnect(Client client) : base(client) { }

        public override void Code() {
            Write(new byte[] {
                0x04, 0x00, 0xf2, 0x03
            });

            Log.Write("[null@{0}] Client connected!", Client.IPAddr);
            Send();
        }
    }
}
