using Estrol.X3Jam.Server.CData;
using System;

namespace Estrol.X3Jam.Server.CHandler {
    public class CMusicList: CBase {
        public CMusicList(Client client) : base(client) { }

        public override void Code() {
            int length = BitConverter.ToInt32(m_client.Message.data, 2);
            byte[] list = new byte[length + 4];
            Buffer.BlockCopy(m_client.Message.data, 6, list, 0, length);

            m_client.UserInfo.MusicLength = length;
            m_client.UserInfo.MusicCount = list;

            Write(new byte[] { 0x04, 0x00, 0xe9, 0x07 });
            Send();
        }
    }
}
