using Estrol.X3Jam.Server.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.Utils {
    public class RoomManager {
        private Room[] Rooms;

        public RoomManager() {
            Rooms = new Room[] {
                new Room(0, "Debug Room #1", new User(new string[] {"null", "debug", "debug"}), 0x01) { SongID = 441, CurrentUser = 3 },
                new Room(2, "Debug Room #2", new User(new string[] {"null", "debug", "debug"}), 0x01, "idk") { SongID = 441, CurrentUser = 4, MaxUser = 4 }
            };
        }

        public int GetNearestEmptyRoomID() {
            int count = 0;
            for (int i = 0; i < 120; i++) {
                Room room = GetRoomByArrayIndex(i);

                if (room?.RoomID == count) {
                    count++;
                    continue;
                }
                break;
            }


            return 0;
        }

        public bool AddRoom(Room room) {
            List<Room> itr = new List<Room>(Rooms) {
                room
            };

            Rooms = itr.ToArray();
            return true;
        }

        public bool RemoveRoom(Room room) {
            List<Room> itr = new List<Room>(Rooms);
            itr.Remove(room);

            Rooms = itr.ToArray();
            return true;
        }

        public Room GetRoomByArrayIndex(int index) {
            if (index + 1 > Rooms.Length) return null;

            return Rooms[index];
        }

        public Room GetRoomById(int ID) {
            for (int i = 0; i < Rooms.Length; i++) {
                if (Rooms[i].RoomID == ID) return Rooms[i];
            }

            return null;
        }

        public Room[] GetRooms() {
            return Rooms;
        }
    }
}
