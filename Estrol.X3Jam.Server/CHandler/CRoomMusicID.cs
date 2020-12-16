using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.Linq;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomMusicID: CBase {
        public CRoomMusicID(Client client) : base(client) { }

        public override void Code() {
            ushort SongID = BitConverter.ToUInt16(m_client.Message.data, 2);
            Room room = m_client.Main.RoomManager.GetID(m_client.UserInfo.Room);

            room.SetSongID(SongID);
            Log.Write("[Channel: {0}, Room: {1}] Set SongID: {2}",
                m_client.UserInfo.ChannelID,
                m_client.UserInfo.Room,
                SongID);

            ushort p_len = (ushort)(m_client.Message.data.Length + 2);
            byte[] len = BitConverter.GetBytes(p_len);
            byte[] data = len.Concat(m_client.Message.data).ToArray();
            data[2] = 0xa1;

            Write(data);
            Send();
        }
    }
}
