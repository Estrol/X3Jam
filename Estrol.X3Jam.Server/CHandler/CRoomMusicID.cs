using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.Linq;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomMusicID: CBase {
        public CRoomMusicID(Client client) : base(client) { }

        public override void Code() {
            ushort SongID = BitConverter.ToUInt16(Client.Message.data, 2);
            Room room = RoomManager.GetID(Client.UserInfo.Room);

            room.SetSongID(SongID);
            Log.Write("[{0}@{1}] (ch: {2}, room: {3}) Set SongID: {4}",
                Client.UserInfo.Username, 
                Client.IPAddr,
                Client.UserInfo.ChannelID,
                Client.UserInfo.Room,
                SongID);

            ushort p_len = (ushort)(Client.Message.data.Length + 2);
            byte[] len = BitConverter.GetBytes(p_len);
            byte[] data = len.Concat(Client.Message.data).ToArray();
            data[2] = 0xa1;

            RoomManager.Send(1, room, room.RoomName, room.Mode);
            Write(data);
            Send();
        }
    }
}
