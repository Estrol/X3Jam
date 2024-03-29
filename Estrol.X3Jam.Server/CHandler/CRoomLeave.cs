﻿using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public class CLeaveRoom: CBase {
        public CLeaveRoom(Client client) : base(client) { }

        public override void Code() {
            Room room = RoomManager.GetID(Client.UserInfo.Room);
            room.RemoveUser(Client.UserInfo);
            Client.UserInfo.Room = -1;

            Write(new byte[] {
                0x08, 0x00, 0xbe, 0x0b, 0x00, 0x00, 0x00, 0x00
            });

            Send();
        }
    }
}
