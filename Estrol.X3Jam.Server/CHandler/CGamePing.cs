using System;
using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public class CGamePing : CBase {
        public CGamePing(Client client) : base(client) { }

        public override void Code() {
            Room room = RoomManager.GetID(Client.UserInfo.Room);
            int slot = room.Slot(Client.UserInfo);

            short flag1 = BitConverter.ToInt16(Client.Message.Data, 2);
            short flag2 = BitConverter.ToInt16(Client.Message.Data, 4);
            int score = BitConverter.ToInt32(Client.Message.Data, 10);
            byte[] positionData = room.Submit(Client.UserInfo, score);

            Write((short)0x0);
            Write((short)0xfaf);
            Write((byte)slot);
            Write(flag1);
            Write(flag2);
            Write(positionData);

            SetL();
            byte[] data = ToArray();

            foreach (User usr in room.GetUsers())
                usr.Connection.Send(data);
        }
    }
}
