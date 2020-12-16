using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomIntialize: CBase {
        public CRoomIntialize(Client client) : base(client) { }

        public override void Code() {
            Write(new byte[] {
                0x06, 0x00, 0xa5, 0x0f, 0x00, 0x00
            });
            Write(new byte[] {
                0x08, 0x00, 0xa3, 0x0f, 0x09, 0x00, 0x00, 0x80,
                0x09, 0x00, 0xb8, 0x0f, 0x01, 0x00, 0x00, 0x00,
                0x00
            });

            Send((short)Stream.Length);
        }
    }
}
