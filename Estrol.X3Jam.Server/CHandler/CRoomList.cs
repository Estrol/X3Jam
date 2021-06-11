using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CUtility;
using Estrol.X3Jam.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomList : CBase {
        public CRoomList(Client client) : base(client) {}

        public override void Code() {
            Room uRoom = RoomManager.GetID(Client.UserInfo.Room);
            if (uRoom != null) {
                uRoom.RemoveUser(Client.UserInfo);
                Client.UserInfo.Room = -1;
            }

            Channel channelById = ChanManager.GetChannelByID(Client.UserInfo.ChannelID);
            User[] users = channelById.GetUsers();

            PacketBuffer buf = new();
            buf.Write((short)0);
            buf.Write((short)2011);
            buf.Write(0);

            foreach (User user in users) {
                buf.Write(user.Info[0]);
                buf.Write(user.Info[1]);
                buf.Write(1);
            }

            buf.SetLength();
            Send(buf.ToArray());

            buf.Stream.SetLength(0L);
            buf.Write((short)0);
            buf.Write((short)2003);
            buf.Write(channelById.m_MaxRoom);
            int num = 0;

            for (int i = 0; i < channelById.m_MaxRoom; ++i) {
                Room room = RoomManager.GetIndex(i);
                if (room != null && room.RoomID == i) {
                    /**
                    * Structure
                    * int32     RoomID
                    * int8      Flag (1 = Waiting, 2 = Playing)
                    * int8[]    Room name (string-null terminated)
                    * int16     OJNID
                    * int8      IsPassword (0 = Nope, 1 = Yes)
                    * int8      Difficulty (0 = EX, 1 = NX, 2 = EX, 3 = RX)
                    * int8      RoomMode   (0 = Single, 1 = VS, 2 = Unknown, 3 = JAM)
                    * int8      Speed!!!   (Speed from 0-8)
                    * int8      Max Players
                    * int8      Current Players
                    * int8      Min Player's Lvl
                    * int8      Max Player's Lvl
                    * int8[8]   Undocumented
                    */

                    buf.Write(room.RoomID);
                    buf.Write((byte)room.IsPlaying);
                    buf.Write(room.RoomName, Encoding.UTF8);
                    buf.Write(room.PasswordFlag);
                    buf.Write((short)room.SongID);
                    buf.Write((byte)room.Difficulty);
                    buf.Write((byte)room.Mode);
                    buf.Write((byte)room.Speed);
                    buf.Write((byte)room.MaxUser);
                    buf.Write((byte)room.CurrentUser);
                    buf.Write((byte)room.MinLvl);
                    buf.Write((byte)room.MaxLvl);
                    buf.Write(new byte[4]);
                    if (room.RingCount > 1) {
                        buf.Write(room.RingCount);
                        buf.Write(room.RingData);
                        buf.Write((short)0);
                    } else {
                        buf.Write((short)0);
                    }

                    ++num;
                } else {
                    buf.Write((byte)8);
                    buf.Write(new byte[11]);
                    buf.Write(byte.MaxValue);
                    buf.Write(new byte[9]);

                }
            }

            buf.Write(byte.MaxValue);
            buf.Write(new byte[11]);
            buf.SetLength();

            Log.Write("[{0}@{1}] Room List. {2} room in total", Client.UserInfo.Username, Client.IPAddr, num);
            Send(buf.ToArray());

            string chat_data = Client.Config.Get("ChannelMessage");
            if (chat_data != null && chat_data != string.Empty) {
                chat_data = chat_data.Replace("{USER}", Client.UserInfo.Username);
                chat_data = chat_data.Replace("{CH}", Client.UserInfo.ChannelID.ToString());

                DateTime date = DateTime.Now;
                chat_data = chat_data.Replace("{TIME}", date.ToString("f"));

                string[] replyList = chat_data.Split('|', StringSplitOptions.RemoveEmptyEntries);
                foreach (string reply in replyList) {
                    byte[] chat = DataUtils.CreateMessage("System", reply);
                    Send(chat);
                }
            }
        }
    }
}
