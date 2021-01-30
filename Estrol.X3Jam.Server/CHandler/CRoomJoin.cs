using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomJoin: CBase {
        public CRoomJoin(Client client) : base(client) { }

        public override void Code() {
            byte roomID = (byte)BitConverter.ToInt16(Client.Message.data, 3);
            Log.Write("[{0}@{1}] Entering room {2}", Client.UserInfo.Username, Client.IPAddr, roomID);

            Room room = RoomManager.GetID(roomID);

            Write((short)0);
            Write((short)0xbbb);
            Write(0);
            Write(roomID);
            Write(room.PasswordFlag);
            Write(room.RoomName);
            Write((short)room.SongID);
            if (room.Arena == RoomArena.Random) {
                Write(new byte[] { 0x12, 0x00, 0x00, 0x80 });
            } else {
                Write((int)room.Arena);
            }
            
            Write((byte)room.Speed);

            int current_slot = room.NearestSlot();
            Write(7);
            for (int i = 0; i < 8; i++) {
                if (current_slot == i) {
                    continue;
                }

                User usr = room.GetUserIndex(i);

                if (usr != null) {
                    int post = room.GetUserPost(usr);

                    Write((byte)post);
                    Write(1); // Ready flag
                    Write(usr.Nickname);
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
            if (room.RingData != null)
                Write(room.RingData);

            Client.UserInfo.Room = roomID;
            room.AddUser(Client.UserInfo);

            SetL();
            Send();
        }
    }
}
