using Estrol.X3Jam.Server.CData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomFinish : CBase {
        public CRoomFinish(Client client) : base(client) { }

        public override void Code() {
            ushort kool = BitConverter.ToUInt16(Client.Message.data, 2);
            ushort great = BitConverter.ToUInt16(Client.Message.data, 4);
            ushort bad = BitConverter.ToUInt16(Client.Message.data, 6);
            ushort miss = BitConverter.ToUInt16(Client.Message.data, 8);
            ushort maxcombo = BitConverter.ToUInt16(Client.Message.data, 10);
            ushort jam = BitConverter.ToUInt16(Client.Message.data, 12);
            ushort pass = BitConverter.ToUInt16(Client.Message.data, 14);
            int score = BitConverter.ToInt32(Client.Message.data, 16);

            Room room = RoomManager.GetID(Client.UserInfo.Room);
            room.SubmitScore(Client.UserInfo, kool, great, bad, miss, maxcombo, jam, pass, score);

            Send(new byte[] {
                0x06, 0x00, 0xb1, 0x0f, 0x00, 0x01
            });
        }
    }
}
