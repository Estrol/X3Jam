using Estrol.X3Jam.Server.CData;
using System;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomJoin: CBase {
        public CRoomJoin(Client client) : base(client) { }

        public override void Code() {
            byte roomID = (byte)BitConverter.ToInt16(m_client.Message.data, 3);
            Write((short)0);
            Write((short)0xbbc);
            Write(roomID);
            Write(m_client.UserInfo.Nickname);

            foreach (int itr in m_client.UserInfo.Char.ToArray()) {
                Write(itr);
            }

            Write(m_client.UserInfo.MusicCount);
            Write(m_client.UserInfo.MusicLength);

            Send();
        }
    }
}
