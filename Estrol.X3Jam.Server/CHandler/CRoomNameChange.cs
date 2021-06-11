using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomNameChange : CBase {
        public CRoomNameChange(Client client) : base(client) { }

        public override void Code() {
            string name = DataUtils.GetString(Client.Message.Data);

            Room room = RoomManager.GetID(Client.UserInfo.Room);
            room.RoomName = name;
            RoomManager.Send(5, room, name);

            Write((short)0);
            Write((short)0xbb9);
            Write(name);
            SetL();

            byte[] data = ToArray();

            foreach (User user in room.GetUsers()) {
                user.Connection.Send(data);
            }
        }
    }
}
