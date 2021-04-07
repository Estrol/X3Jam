using Estrol.X3Jam.Server.CData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CInGameExit : CBase {
        public CInGameExit(Client client) : base (client) { }

        public override void Code() {
            Room room = RoomManager.GetID(Client.UserInfo.Room);
            int slot = room.Slot(Client.UserInfo);

            Write((short)0x09);
            Write((short)0xfb6);
            Write((byte)slot);
            Write(Client.UserInfo.Level);

            if (room.IsPlaying == RoomStatus.Playing) {
                room.SubmitScore(Client.UserInfo, 0, 0, 0, 0, 0, 0, 0, 0);
                room.RemoveUser(Client.UserInfo);

                room.Event(7, null, slot);
            }

            Send();
        }
    }
}
