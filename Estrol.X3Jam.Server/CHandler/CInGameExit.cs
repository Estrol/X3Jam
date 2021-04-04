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
            Write((short)0x09);
            Write((short)0xfb6);
            Write((byte)0);

            if (room.IsPlaying == RoomStatus.Playing) {
                int slot = room.Slot(Client.UserInfo);
                room.SubmitScore(Client.UserInfo, 0, 0, 0, 0, 0, 0, 0, 0);
                room.RemoveUser(Client.UserInfo);

                Write(Client.UserInfo.Level);

                room.Event(7, null, slot);
            } else {
                Write(Client.UserInfo.Level);
            }

            Send();
        }
    }
}
