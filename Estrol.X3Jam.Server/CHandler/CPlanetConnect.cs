using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public class CPlanetConnect: CBase {
        public CPlanetConnect(Client client) : base(client) { }

        public override void Code() {
            Write(new byte[] {
                0x04, 0x00, 0xf4, 0x03
            });

            Send();
        }
    }
}
