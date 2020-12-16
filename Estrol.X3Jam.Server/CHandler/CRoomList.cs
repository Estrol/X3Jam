using System.IO;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.Utils;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomList: CBase {
        public CRoomList(Client client) : base(client) { }

        public override void Code() {
            Channel channel = m_client.Main.ChannelManager.GetChannelByID(m_client.UserInfo.ChannelID);
            User[] users = channel.GetUsers();

            PBuffer buf = new PBuffer();
            buf.WriteS(0);
            buf.WriteS(0x7db);
            buf.WriteI(0);

            foreach (User user in users) {
                buf.WriteStr(user.Info[0]);
                buf.WriteStr(user.Info[1]);
                buf.WriteI(1);
            }

            buf.SetL();
            Write(buf.ToArray());

            PBuffer buf2 = new PBuffer();
            buf2.WriteS(0);
            buf2.WriteS(0x7d3);
            buf2.WriteI(120);

            int count = 0;
            for (int i = 0; i < 120; i++) {
                Room room = m_client.Main.RoomManager.GetIndex(count);

                if (room != null && room.RoomID == i) {
                    buf2.WriteI(room.RoomID);
                    buf2.WriteBB(0x02);
                    buf2.WriteStr(room.RoomName);
                    buf2.WriteBB(0x10);
                    buf2.WriteBB(0x00);
                    buf2.WriteBB(room.PasswordFlag);
                    buf2.WriteI(room.SongID);
                    buf2.WriteBB(0x00);
                    buf2.WriteBB((byte)room.MaxUser);
                    buf2.WriteBB((byte)room.MaxUser);
                    buf2.WriteL(0);

                    count++;
                } else {
                    buf2.WriteBB(8);
                    buf2.WriteBA(new byte[11]);
                    buf2.WriteBB(0xff);
                    buf2.WriteBA(new byte[9]);
                }
            }

            buf2.WriteBB(0xff);
            buf2.WriteBA(new byte[11]);
            buf2.SetL();
            Write(buf2.ToArray());

            Send((short)Stream.Length);
        }
    }
}
