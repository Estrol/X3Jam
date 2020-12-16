using Estrol.X3Jam.Server.CData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CManager {
    public class RoomManager {
        private Room[] Rooms;

        public RoomManager() {
            Rooms = new Room[] {};
        }

        public int EmptyID() {
            int count = 0;
            for (int i = 0; i < 120; i++) {
                Room room = GetIndex(i);

                if (room?.RoomID == count) {
                    count++;
                    continue;
                }
                break;
            }


            return 0;
        }

        public bool Add(Room room) {
            List<Room> itr = new List<Room>(Rooms) {
                room
            };

            Rooms = itr.ToArray();
            return true;
        }

        public bool Remove(Room room) {
            List<Room> itr = new List<Room>(Rooms);
            itr.Remove(room);

            Rooms = itr.ToArray();
            return true;
        }

        public Room GetIndex(int index) {
            if (index + 1 > Rooms.Length) return null;

            return Rooms[index];
        }

        public Room GetID(int ID) {
            for (int i = 0; i < Rooms.Length; i++) {
                if (Rooms[i].RoomID == ID) return Rooms[i];
            }

            return null;
        }

        public Room[] RoomList() {
            return Rooms;
        }
    }
}
