using Estrol.X3Jam.Server.CData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CGameReady : CBase {
        public CGameReady(Client client) : base(client) { }

        public override void Code() {
            Room room = RoomManager.GetID(Client.UserInfo.Room);
            int slot = room.Slot(Client.UserInfo);

            Write((short)0);
            Write((short)0xfad);
            Write((byte)slot); // Could be this?
            Write(room.RingData ?? (new byte[2]));
            SetL();
            byte[] data = ToArray();

            foreach (User user in room.GetUsers())
                user.Connection.Send(data);
        }
    }
}
