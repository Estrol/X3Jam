using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.Linq;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomMusicID: CBase {
        public CRoomMusicID(Client client) : base(client) { }

        public override void Code() {
            ushort SongID = BitConverter.ToUInt16(Client.Message.Data, 2);
            int Diff = Client.Message.Data[4];
            int Speed = Client.Message.Data[5];

            Room room = RoomManager.GetID(Client.UserInfo.Room);

            room.SetSongID(SongID, Diff, Speed);
            Log.Write("[{0}@{1}] (ch: {2}, room: {3}) Set SongID: {4}",
                Client.UserInfo.Username, 
                Client.IPAddr,
                Client.UserInfo.ChannelID,
                Client.UserInfo.Room,
                SongID);

            //RoomManager.Send(1, room, room.RoomName, (int)room.IsPlaying);
            RoomManager.Send(4, room);
        }
    }
}
