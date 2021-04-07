using Estrol.X3Jam.Server.CData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomUserReady : CBase {
        public CRoomUserReady(Client client) : base(client) { }

        public override void Code() {
            Room room = RoomManager.GetID(Client.UserInfo.Room);
            int slot = room.Slot(Client.UserInfo);

            if (Client.UserInfo.Ready == 1) {
                Client.UserInfo.Ready = 0;

                Write(new byte[] {
                    0x06, 0x00,
                    0xa9, 0x0f,
                    (byte)slot, 0x00
                });

            } else {
                Client.UserInfo.Ready = 1;

                Write(new byte[] {
                    0x06, 0x00,
                    0xa9, 0x0f,
                    (byte)slot, 0x01
                });
            }

            byte[] data = ToArray();
            foreach (User usr in room.GetUsers())
                usr.Connection.Send(data);
        }
    }
}
