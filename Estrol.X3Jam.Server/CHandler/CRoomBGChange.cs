using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.IO;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomBGChange : CBase {
        public CRoomBGChange(Client client) : base(client) { }

        public override void Code() {
            byte[] data = new byte[4];
            Buffer.BlockCopy(Client.Message.data, 2, data, 0, 4);

            RoomArena arena;
            if (data[3] == 0x80) {
                arena = RoomArena.Random;
            } else {
                arena = (RoomArena)BitConverter.ToInt32(data);
            }

            Client.Message.full_data[2] = 0xa3;
            Room room = RoomManager.GetIndex(Client.UserInfo.Room);
            room.RandomArenaNumber = data[0];
            room.Arena = arena;

            Log.Write("[{0}@{1}] (ch: {2}, room: {3}) Set ArenaID: {4}",
                Client.UserInfo.Username,
                Client.IPAddr,
                Client.UserInfo.ChannelID,
                Client.UserInfo.Room,
                arena.ToString());

            foreach (User usr in room.GetUsers())
                usr.Connection.Send(Client.Message.full_data);
        }
    }
}
