using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Server.Utils;
using System.IO;
using System.Text;

namespace Estrol.X3Jam.Server.Handlers {
    public class GetRoom : Base {
        public GetRoom(ClientSocket state, PacketManager PM, ServerMain _back) : base(state) { 
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            var Channel = _back.ChannelMG.GetChannelByID(state.UserInfo.ChannelID);
            var ChannelUsers = Channel.GetUsers();
            bw.Write((short)0x0); // len header
            bw.Write(new byte[] { 0xdb, 0x07 }); // opcode
            bw.Write(1);

            for (int i = 0; i < ChannelUsers.Length; i++) {
                string[] info = ChannelUsers[i].Info;
                bw.Write(Encoding.UTF8.GetBytes(info[0].ToCharArray()));
                bw.Write(new byte[1]);
                bw.Write(Encoding.UTF8.GetBytes(info[1].ToCharArray()));
                bw.Write(new byte[1]);

                bw.Write(1);
            }

            bw.Seek(0, SeekOrigin.Begin);
            bw.Write((short)ms.Length);
            Write(ms.ToArray());

            var ms3 = new MemoryStream();
            var bw3 = new BinaryWriter(ms3);

            bw3.Write((short)0x0); // len header
            bw3.Write(new byte[] { 0xd3, 0x07 }); // opcode
            bw3.Write(120);

            int roomCount = 0;
            for (int i = 0; i < 120; i++) {
                Room room = _back.RoomMG.GetRoomByArrayIndex(roomCount);

                if (room != null && room.RoomID == i) {
                    bw3.Write(room.RoomID);
                    bw3.Write((byte)0x02); // Playing or Waiting flag
                    bw3.Write(Encoding.UTF8.GetBytes(room.RoomName.ToCharArray()));
                    bw3.Write((byte)0x10);
                    bw3.Write((byte)0x00);
                    bw3.Write(room.PasswordFlag);
                    bw3.Write(room.SongID);
                    bw3.Write((byte)0x00);
                    bw3.Write((byte)room.MaxUser); // Room max user
                    bw3.Write((byte)room.CurrentUser); // Room current user
                    bw3.Write(new byte[8]);

                    roomCount++;
                } else {
                    bw3.Write((byte)0x08);
                    bw3.Write(new byte[11]);

                    bw3.Write((byte)0xff);
                    bw3.Write(new byte[9]);
                }
            }

            bw3.Write((byte)0xff);
            bw3.Write(new byte[11]);
            bw3.Seek(0, SeekOrigin.Begin);
            bw3.Write((short)ms3.Length);
            Write(ms3.ToArray());

            //Write(Properties.Resources.EmuRoomList);
            Send((short)base.m_MemoryStream.Length);
        }
    }
}
