using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CData.RoomEnums;
using Estrol.X3Jam.Server.CUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CPlayerColorChange : CBase {
        public CPlayerColorChange(Client client) : base(client) { }

        public override void Code() {
            byte color = Client.Message.data[2];
            Room room = RoomManager.GetID(Client.UserInfo.Room);
            int slot = room.Slot(Client.UserInfo);

            Client.UserInfo.Color = (RoomColor)color;

            Write(new byte[] {
                0x06, 0x00, 0xa5, 0x0f, (byte)slot, color
            });

            Write(new byte[] {
                0x06, 0x00, 0xa9, 0x0f, (byte)slot, 0x01
            });

            byte[] data = ToArray();
            foreach (User usr in room.GetUsers())
                usr.Connection.Send(data);
        }
    }
}
