using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Utility.Data;
using System;
using System.IO;
using System.Linq;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomJoin: CBase {
        public CRoomJoin(Client client) : base(client) { }

        public override void Code() {
            byte roomID = (byte)BitConverter.ToInt16(Client.Message.Data, 3);
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
                Write(new byte[] { (byte)room.RandomArenaNumber, 0x00, 0x00, 0x80 });
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
                    Write((byte)usr.Char.Gender); // Gender
                    Write((byte)(usr.IsRoomMaster ? 1 : 0)); // Roomaster
                    Write((byte)usr.Color); // Room Color
                    Write((byte)usr.Ready); // Ready?
                    Write((byte)0);

                    Write(usr.Char.Instrument); // 0
                    Write(usr.Char.Hair); // 1
                    Write(usr.Char.Accessory); // 2
                    Write(usr.Char.Glove); // 3
                    Write(usr.Char.Necklace); // 4
                    Write(usr.Char.Cloth); // 5
                    Write(usr.Char.Pant); // 6
                    Write(usr.Char.Glass); // 7
                    Write(usr.Char.Earring); // 8
                    Write(usr.Char.ClothAccessory); // 9
                    Write(usr.Char.Shoe); // 10
                    Write(usr.Char.Face); // 11
                    Write(usr.Char.Wing); // 12
                    Write(usr.Char.InstrumentAccessory); // 13
                    Write(usr.Char.Pet); // 14
                    Write(usr.Char.HairAccessory); // 15

                    Write(usr.MusicLength);
                    Write(usr.MusicCount);
                } else {
                    bool IsLocked = !room.ListSlot[i];

                    Write((byte)i);
                    Write(IsLocked ? 2 : 0); // 0 = Not locked, 2 = Locked (based on research)
                }
            }

            // Apply ring
            if (room.RingData != null) { 
                if (room.RingCount < 1) {
                    Write(0);
                } else {
                    Write(room.RingCount);
                    Write(room.RingData);
                    Write((short)0);
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
