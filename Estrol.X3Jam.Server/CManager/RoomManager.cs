using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CUtility;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CManager {
    public class RoomManager {
        public O2JamServer Server;
        public Channel Channel;
        private Dictionary<int, Room> Rooms;
        private int MaxRoom;

        public RoomManager(O2JamServer svr, Channel ch, int _MaxRoom) {
            Channel = ch;
            Server = svr;

            Rooms = new(_MaxRoom);
            MaxRoom = _MaxRoom;
        }

        public int Count => Rooms.Count;

        public int EmptyID() {
            int count = 0;
            for (int i = 0; i < MaxRoom; i++) {
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

            Send(1, room, room.RoomName, (int)room.Mode);
            return true;
        }

        public bool Remove(Room room) {
            var itr = Rooms.First(x => x.Value == room);
            Rooms.Remove(itr.Key);

            Send(2, room);
            return true;
        }

        public Room GetIndex(int index) {
            if (index + 1 > Rooms.Count) return null;

            return Rooms.GetValue(index);
        }

        public Room GetID(int ID) {
            var value = Rooms.GetValue(ID);

            return value;
        }

        public Room[] RoomList() {
            return Rooms.Values.ToList().ToArray();
        }

        public void Send(int idcase, Room room, string name = "", int flag = 0, int max = 0, int current = 0) {
            User[] users = Channel.GetUsers();

            switch (idcase) {
                case 1: { // Room Add
                    PacketBuffer buf = new();
                    buf.Write((short)0);
                    buf.Write((short)0x7d5);
                    buf.Write(room.RoomID);
                    buf.Write(name);
                    buf.Write(flag); // 0 = Solo, 1 = VS, 3 = Coop
                    buf.Write((short)0);
                    

                    PacketBuffer buf2 = new();
                    buf2.Write((short)0);
                    buf2.Write((short)0x7e9);
                    buf2.Write(room.RoomID);
                    if (room.RingCount > 1) {
                        buf2.Write(room.RingCount);
                        buf2.Write(room.RingData);
                        buf2.Write((short)0);
                    } else {
                        buf2.Write(0);
                    }

                    buf.SetLength();
                    buf2.SetLength();
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
                    buf.Write((short)0x7e7);
                    buf.Write(room.RoomID);
                    buf.Write((short)room.SongID);
                    buf.Write((byte)room.Difficulty);
                    buf.Write((byte)room.Speed);
                    

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
                    buf.Write((short)0);
                    buf.Write((short)0x77e);
                    buf.Write(room.RoomID);
                    buf.Write(name);
                    buf.SetLength();

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

                    buf.Write((short)0);
                    buf.Write((short)0x77e);
                    buf.Write(room.RoomID);
                    if (room.RingCount > 1) {
                        buf.Write(room.RingCount);
                        buf.Write(room.RingData);
                        buf.Write((short)0);
                    } else {
                        buf.Write(0);
                    }

                    buf.SetLength();
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
