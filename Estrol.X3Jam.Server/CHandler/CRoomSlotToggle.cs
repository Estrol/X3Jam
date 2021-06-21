using Estrol.X3Jam.Server.CData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomSlotToggle : CBase {
        public CRoomSlotToggle(Client client) : base(client) { }

        public override void Code() {
            Room room = RoomManager.GetID(Client.UserInfo.Room);

            var slot = (int)Client.Message.Data[2];
            var status = room.ListSlot[slot] ? SlotStatus.Close : SlotStatus.Open;

            room.ListSlot[slot] = !room.ListSlot[slot];

            Write((short)0);
            Write((short)0xbc1);
            Write((byte)slot);
            Write((byte)status);
            SetL();

            byte[] data = ToArray();
            foreach (User usr in room.GetUsers())
                usr.Connection.Send(data);
        }
    }

    public enum SlotStatus {
        Open = 0,
        Kick = 1,
        Close = 2
    }
}
