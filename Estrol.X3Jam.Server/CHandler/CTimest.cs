using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public class CTimest: CBase {
        public CTimest(Client client) : base(client) { }

        public override void Code() {
            Write(new byte[] {
                0x08, 0x00, 0xa5, 0x13, 0xc6,
                0xf5, 0x02, 0x00
            });

            Send();
        }
    }
}
