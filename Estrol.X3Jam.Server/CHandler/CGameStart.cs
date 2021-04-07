using Estrol.X3Jam.Server.CData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CGameStart : CBase {
        public CGameStart(Client client) : base(client) { }

        public override void Code() {
            Room room = RoomManager.GetID(Client.UserInfo.Room);
            if (!room.IsReady()) {
                Write(new byte[] {

                });

                Send();
                return;
            }

            room.GamePrepare();

            foreach (User usr in room.GetUsers())
                usr.Connection.Send(new byte[] {
                    0x0c, 0x00, 0xab, 0x0f, 0x00, 0x00, 0x00, 0x00,
                    0x093, 0x21, 0x74, 0x025
                });
        }
    }
}
