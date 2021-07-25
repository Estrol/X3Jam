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
                    0x08, 0x00, 0xab, 0x0f, 
                    0x01, 0x00, 0x00, 0x00
                });

                Send();
                return;
            }

            room.GamePrepare();

            Random rnd = new();
            int timeRandom = rnd.Next(0, int.MaxValue);

            Write(new byte[] { 0x0c, 0x00, 0xab, 0x0f, 0x00, 0x00, 0x00, 0x00 });
            Write(timeRandom); // this needed to make random ring work for each play

            byte[] data = ToArray();
            foreach (User usr in room.GetUsers())
                usr.Connection.Send(data);
        }
    }
}
