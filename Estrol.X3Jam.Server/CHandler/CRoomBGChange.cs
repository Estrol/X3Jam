using Estrol.X3Jam.Server.CData;
using System;
using System.IO;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomBGChange : CBase {
        public CRoomBGChange(Client client) : base(client) { }

        public override void Code() {
            using MemoryStream ms = new(Client.Message.data);
            using BinaryReader br = new(ms);

            /* Discard */ br.ReadInt16();

            RoomArena arena = (RoomArena)br.ReadInt32();
            Client.Message.full_data[2] = 0xa3;

            Room room = RoomManager.GetIndex(Client.UserInfo.Room);
            room.Arena = arena;

            foreach (User usr in room.GetUsers())
                usr.Connection.Send(Client.Message.full_data);
        }
    }
}
