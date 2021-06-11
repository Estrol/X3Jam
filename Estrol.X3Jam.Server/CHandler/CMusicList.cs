using System;

using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public class CMusicList: CBase {
        public CMusicList(Client client) : base(client) { }

        public override void Code() {
            int length = BitConverter.ToInt32(Client.Message.Data, 2);
            byte[] list = new byte[length * 2];
            Buffer.BlockCopy(Client.Message.Data, 6, list, 0, length * 2);

            Client.UserInfo.MusicLength = length;
            Client.UserInfo.MusicCount = list;

            Write(new byte[] { 0x04, 0x00, 0xe9, 0x07 });
            Log.Write("[{0}@{1}] Client Music List: {2} Songs", Client.UserInfo.Username, Client.IPAddr, length);
            Send();
        }
    }
}
