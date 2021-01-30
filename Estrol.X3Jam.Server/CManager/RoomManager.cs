using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CUtility;
using Estrol.X3Jam.Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CManager {
    public class RoomManager {
        private O2JamServer Server;
        private Channel Channel;
        private Dictionary<int, Room> Rooms;

        public RoomManager(O2JamServer svr, Channel ch) {
            Channel = ch;
            Server = svr;

            Rooms = new(120);
        }

        public int Count => Rooms.Count;

        public int EmptyID() {
            int count = 0;
            for (int i = 0; i < 120; i++) {
                Room room = GetIndex(i);

                if (room?.RoomID == i) {
                    continue;
                } else {
                    count = i;
                    break;
                }
                
            }

            return count;
        }

        public bool Add(Room room) {
            int pos = EmptyID();
            Rooms.Add(pos, room);

            //Send(1, room, room.RoomName, room.Mode);
            return true;
        }

        public bool Remove(Room room) {
            var itr = Rooms.First(x => x.Value == room);
            Rooms.Remove(itr.Key);

            Send(2, room);
            return true;
        }

        public Room GetIndex(int index) {
            if (index > Rooms.Count) return null;

            return Rooms.GetValue(index);
        }

        public Room GetID(int ID) {
            var value = Rooms.GetValue(ID);

            return value;
        }

        public Room[] RoomList() {
            List<Room> itr_list = new();

            foreach (KeyValuePair<int, Room> itr in Rooms)
                itr_list.Add(itr.Value);

            return itr_list.ToArray();
        }

        public void Send(int idcase, Room room, string name = "", int flag = 0, int max = 0, int current = 0) {
            User[] users = Channel.GetUsers();

            switch (idcase) {
                case 1: { // Room Add
                    PacketBuffer buf = new();
                    short length = (short)(8 + name.Length + 5);
                    buf.Write(length);
                    buf.Write((short)0x7d5);
                    buf.Write(room.RoomID);
                    buf.Write(name);
                    buf.Write(flag); // 0 = Solo, 1 = VS, 3 = Coop
                    buf.Write((short)0);

                    PacketBuffer buf2 = new();
                    buf2.Write((short)0xc);
                    buf2.Write((short)0x77e);
                    buf2.Write(room.RoomID);
                    buf2.Write(room.SongID);

                    byte[] data = buf.ToArray();
                    byte[] data2 = buf2.ToArray();
                    foreach (User user in users) {
                        if (user.Room == -1) {
                            user.Connection.Send(data);
                            user.Connection.Send(data2);
                        }
                    }

                    break;
                }

                case 2: { // Room Remove
                    PacketBuffer buf = new();
                    buf.Write((short)0x08);
                    buf.Write((short)0x7d7);
                    buf.Write(room.RoomID);

                    byte[] data = buf.ToArray();
                    foreach (User user in users) {
                        if (user.Room == -1) {
                            user.Connection.Send(data);
                        }
                    }

                    break;
                }

                case 3: { // Room status toggle
                    PacketBuffer buf = new();
                    buf.Write((short)0x09);
                    buf.Write((short)0x7e4);
                    buf.Write(room.RoomID);
                    buf.Write((byte)flag); // 1 = Waiting, 2 = Playing

                    byte[] data = buf.ToArray();
                    foreach (User user in users) {
                        if (user.Room == -1) {
                            user.Connection.Send(data);
                        }
                    }

                    break;
                }

                case 4: { // Room OJN ID Change
                    PacketBuffer buf = new();
                    buf.Write((short)0xc);
                    buf.Write((short)0x77e);
                    buf.Write(room.RoomID);
                    buf.Write(room.SongID);

                    byte[] data = buf.ToArray();
                    foreach (User user in users) {
                        if (user.Room == -1) {
                            user.Connection.Send(data);
                        }
                    }

                    break;
                }

                case 5: { // Room Name
                    PacketBuffer buf = new();
                    short length = (short)(8 + name.Length + 1);

                    buf.Write(length);
                    buf.Write((short)0x77e);
                    buf.Write(room.RoomID);
                    buf.Write(name);

                    byte[] data = buf.ToArray();
                    foreach (User user in users) {
                        if (user.Room == -1) {
                            user.Connection.Send(data);
                        }
                    }

                    break;
                }

                case 6: { // Room's Rings
                    PacketBuffer buf = new();
                    short length = (short)(8 + (room.RingData != null ? room.RingData.Length : 4));

                    buf.Write(length);
                    buf.Write((short)0x77e);
                    buf.Write(room.RoomID);
                    if (room.RingData != null) {
                        buf.Write(room.RingCount);
                        buf.Write(room.RingData);
                    } else {
                        buf.Write(0);
                    }

                    byte[] data = buf.ToArray();
                    foreach (User user in users) {
                        if (user.Room == -1) {
                            user.Connection.Send(data);
                        }
                    }

                    break;
                }

                case 7: { // Room's max user changes
                    PacketBuffer buf = new();
                    buf.Write((short)0x0c);
                    buf.Write((short)0x207);
                    buf.Write(room.RoomID);
                    buf.Write((byte)room.MaxUser);
                    buf.Write((short)1);
                    buf.Write((byte)0);

                    byte[] data = buf.ToArray();
                    foreach (User user in users) {
                        if (user.Room == -1) {
                            user.Connection.Send(data);
                        }
                    }

                    break;
                }

                default: {
                    break;
                }
            }
        }
    }
}
