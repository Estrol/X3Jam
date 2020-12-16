using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public class CTcpPing: CBase {
        public CTcpPing(Client client) : base(client) { }

        public override void Code() {
            Write(new byte[] { 0x04, 0x00, 0x71, 0x17 });
            Send();
        }
    }
}
