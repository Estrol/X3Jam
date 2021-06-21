using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class COJNList: CBase {
        public COJNList(Client client) : base(client) { }

        public override void Code() {
            Channel ch = Main.ChannelManager.GetChannelByID(Client.UserInfo.ChannelID);
            OJN[] headers = ch.GetMusicList();

            short length = (short)(6 + (headers.Length * 12) + 12);
            Write(length);
            Write((short)0xfbf);
            Write((short)headers.Length);

            foreach (OJN ojn in headers) {
                Write((short)ojn.Id);
                Write((short)ojn.NoteCountEx);
                Write((short)ojn.NoteCountNx);
                Write((short)ojn.NoteCountHx);
                Write(0);
            }

            Write((long)0);
            Write(0);

            Log.Write("[{0}@{1}] Server Music List", Client.UserInfo.Username, Client.IPAddr);
            Send();
        }
    }
}
