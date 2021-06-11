using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomFinish : CBase {
        public CRoomFinish(Client client) : base(client) { }

        public override void Code() {
            ushort kool = BitConverter.ToUInt16(Client.Message.Data, 2);
            ushort great = BitConverter.ToUInt16(Client.Message.Data, 4);
            ushort bad = BitConverter.ToUInt16(Client.Message.Data, 6);
            ushort miss = BitConverter.ToUInt16(Client.Message.Data, 8);
            ushort maxcombo = BitConverter.ToUInt16(Client.Message.Data, 10);
            ushort jam = BitConverter.ToUInt16(Client.Message.Data, 12);
            ushort pass = BitConverter.ToUInt16(Client.Message.Data, 14);
            int score = BitConverter.ToInt32(Client.Message.Data, 16);

            //Log.Write(BitConverter.ToString(Client.Message.data).Replace("-", " "));

            Room room = RoomManager.GetID(Client.UserInfo.Room);
            room.SubmitScore(Client.UserInfo, kool, great, bad, miss, maxcombo, jam, pass, score);

            // 0x01 = User passed the anti-cheat
            // 0x00 = User not passed the anti-cheat => client exit
            Send(new byte[] {
                0x06, 0x00, 0xb1, 0x0f, 0x00, 0x01
            });
        }
    }
}
