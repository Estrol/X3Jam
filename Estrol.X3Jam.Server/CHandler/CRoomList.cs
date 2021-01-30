using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Text;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CUtility;
using Estrol.X3Jam.Server.Utils;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomList: CBase {
        public CRoomList(Client client) : base(client, false) { }

        public override void Code() {
            Channel channel = ChanManager.GetChannelByID(Client.UserInfo.ChannelID);
            User[] users = channel.GetUsers();

            PacketBuffer buf = new(); // First Stream
            buf.Write((short)0);
            buf.Write((short)0x7db);
            buf.Write(0);

            foreach (User user in users) {
                buf.Write(user.Info[0]);
                buf.Write(user.Info[1]);
                buf.Write(1);
            }

            buf.SetLength();
            Send(buf.ToArray());

            buf.Stream.SetLength(0); // Reset Stream;
            buf.Write((short)0);
            buf.Write((short)0x7d3);
            buf.Write(channel.m_MaxRoom);

            for (int i = 0; i < channel.m_MaxRoom; i++) {
                Room room = RoomManager.GetIndex(i);

                if (room != null && room.RoomID == i) {
                    buf.Write(room.RoomID);
                    buf.Write((byte)room.IsPlaying); //waiting or playing
                    buf.Write(room.RoomName, Encoding.UTF8);
                    buf.Write((byte)0x00);
                    buf.Write((short)room.SongID);
                    buf.Write((byte)0x00);
                    buf.Write((byte)room.MaxUser);
                    buf.Write((byte)room.CurrentUser);
                    buf.Write(new byte[8]);
                } else {
                    buf.Write((byte)i);
                    buf.Write(new byte[11]);
                    buf.Write((byte)0xff);
                    buf.Write(new byte[9]);
                }
            }

            buf.Write((byte)0xff);
            buf.Write(new byte[11]);
            buf.SetLength();

            Log.Write("[{0}@{1}] Room List.", Client.UserInfo.Username, Client.IPAddr);
            Send(buf.ToArray());
        }
    }
}
