using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.IO;
using System.Linq;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomJoin: CBase {
        public CRoomJoin(Client client) : base(client) { }

        public override void Code() {
            byte roomID = (byte)BitConverter.ToInt16(Client.Message.data, 3);
            Log.Write("[{0}@{1}] Entering room {2}", Client.UserInfo.Username, Client.IPAddr, roomID);

            Room room = RoomManager.GetID(roomID);
            int position = room.NearestSlot();
            var Color = room.GetEmptyColor();

            Write((short)0);
            Write((short)0xbbb);
            Write(room.RoomID);
            Write((byte)position);  // todo: fix those functions....
            Write((byte)Color);
            Write(room.RoomName);
            Write((short)room.SongID);
            if (room.Arena == RoomArena.Random) {
                Random rand = new();
                int _room = rand.Next(0, 12);

                Write(new byte[] { (byte)_room, 0x00, 0x00, 0x80 });
            } else {
                Write((int)room.Arena);
            }
            
            Write((byte)room.Mode); // 0 = Solo, 1 = VS, 2 = Tournament, 3 = JAM
            Write((byte)room.Difficulty);
            Write((byte)room.Speed);
            Write(7); // idk this
            for (int i = 0; i < 8; i++) {
                if (position == i) {
                    continue;
                }

                User usr = room.GetUserIndex(i);

                if (usr != null) {
                    int post = room.GetUserPost(usr);

                    Write((byte)post);
                    Write(1);
                    Write(usr.Nickname);
                    Write(usr.Level);
                    Write((byte)0);
                    Write((byte)1); // Roomaster
                    Write((byte)usr.Color); // Room Color
                    Write((byte)0); // Ready?
                    Write((byte)0); 
                    foreach (int itr in usr.Char.ToArray()) {
                        Write(itr);
                    }

                    Write(usr.MusicLength);
                    Write(usr.MusicCount);
                } else {
                    Write((byte)i);
                    Write(0);
                }
            }

            // Apply ring
            if (room.RingData != null) { 
                if (room.RingCount < 1) {
                    Write(0);
                } else {
                    Write(room.RingData);
                }
            } else {
                Write(0);
            } 

            Client.UserInfo.Room = roomID;
            room.AddUser(Client.UserInfo, Color);

            SetL();
            Send();
        }
    }
}
