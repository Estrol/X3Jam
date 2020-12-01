using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Server.Utils;
using System.IO;
using System.Text;

namespace Estrol.X3Jam.Server.Handlers {
    public class GetRoom : Base {
        public GetRoom(Connection state, PacketManager PM, ServerMain _back) : base(state) {
            Write(this.CreateWarningMessage(state));

            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            var Channel = _back.ChannelMG.GetChannelByID(state.UserInfo.GetChannel());
            var ChannelUsers = Channel.GetUsers();
            bw.Write((short)0x0); // len header
            bw.Write(new byte[] { 0xdb, 0x07 }); // opcode
            bw.Write(1);

            for (int i = 0; i < ChannelUsers.Length; i++) {
                string[] info = ChannelUsers[i].GetInfo();
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

        public byte[] CreateWarningMessage(Connection state) {
            var FinalStream = new MemoryStream();
            var FinalBW = new BinaryWriter(FinalStream);

            var MS1 = new MemoryStream();
            var BW1 = new BinaryWriter(MS1);
            if (true) { // Useless context idk why
                BW1.Write((short)0x0); // len header
                BW1.Write(new byte[] { 0xdd, 0x07 }); // opcode
                BW1.Write(new byte[] { 0x8c, 0x6e, 0x3f, 0x8f, 0xc1, 0x91, 0xa7, 0x00 });

                string Message = string.Format("{0}, Welcome to X3-JAM, 1.8 Server Emulation Alpha", state.UserInfo.GetUsername());
                char[] charArry = Message.ToCharArray();
                BW1.Write(Encoding.UTF8.GetBytes(charArry));
                BW1.Write(new byte[] {
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x0, 0x0
                });

                BW1.Seek(0, SeekOrigin.Begin);
                BW1.Write((short)MS1.Length);
                FinalBW.Write(MS1.ToArray());
            }

            var MS2 = new MemoryStream();
            var BW2 = new BinaryWriter(MS2);
            if (true) { // Useless context idk why
                BW2.Write((short)0x0); // len header
                BW2.Write(new byte[] { 0xdd, 0x07 }); // opcode
                BW2.Write(new byte[] { 0x8c, 0x6e, 0x3f, 0x8f, 0xc1, 0x91, 0xa7, 0x00 });

                string Message = string.Format("Be aware that this server emulator still alpha.", state.UserInfo.GetUsername());
                char[] charArry = Message.ToCharArray();
                BW2.Write(Encoding.UTF8.GetBytes(charArry));
                BW2.Write(new byte[] {
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x0, 0x0
                });

                BW2.Seek(0, SeekOrigin.Begin);
                BW2.Write((short)MS2.Length);
                FinalBW.Write(MS2.ToArray());
            }

            var MS3 = new MemoryStream();
            var BW3 = new BinaryWriter(MS3);
            if (true) { // Useless context idk why
                BW3.Write((short)0x0); // len header
                BW3.Write(new byte[] { 0xdd, 0x07 }); // opcode
                BW3.Write(new byte[] { 0x8c, 0x6e, 0x3f, 0x8f, 0xc1, 0x91, 0xa7, 0x00 });

                string Message = string.Format("Expect bugs or whatever.", state.UserInfo.GetUsername());
                char[] charArry = Message.ToCharArray();
                BW3.Write(Encoding.UTF8.GetBytes(charArry));
                BW3.Write(new byte[] {
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20,
                    0x0, 0x0
                });

                BW3.Seek(0, SeekOrigin.Begin);
                BW3.Write((short)MS3.Length);
                FinalBW.Write(MS3.ToArray());
            }

            return FinalStream.ToArray();
        }
    }
}
