using Estrol.X3Jam.Server.CManager;
using Estrol.X3Jam.Server.CUtility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CData {
    public class Room {
        public RoomManager RoomManager;
        private Dictionary<int, User> Users;

        public string RoomName;
        public string Password;
        public RoomArena Arena;
        public RoomSpeed Speed;
        public RoomStatus IsPlaying;

        public byte PasswordFlag;
        public byte[] RingData;
        public int RingCount;
        public bool WaitForSong;

        public int CurrentUser;
        public int MaxUser;
        public int RoomID;
        public int RoomBG;
        public int SongID;
        public int Mode;
        public int MinLvl;
        public int MaxLvl;

        public Room(RoomManager rm, int _RoomID, string Name, int ojn, User user, byte flag, string password = "", int mode = 0) {
            RoomManager = rm;

            RoomName = Name;
            RoomID = _RoomID;

            Users = new(8);
            Users.Add(0, user);

            PasswordFlag = flag;
            Password = password;

            WaitForSong = true;
            Arena = RoomArena.Random;
            Speed = RoomSpeed.Speed10; 
            IsPlaying = RoomStatus.Waiting; // 1: Waiting, 2: Playing
            Mode = 0; // 0: Solo, 1: VS
            MaxUser = 8;
            CurrentUser = 1;
            MinLvl = 0;
            MaxLvl = 0;

            SongID = ojn; // For DEBUG Purpose only
        }

        public void SetSongID(int ID) {
            SongID = ID;
        }

        public bool CheckPassword(string passwd) {
            return passwd == Password;
        }

        public int GetNearestEmpty() {
            int found = 0;

            for (int i = 0; i < 8; i++) {
                try {
                    var itr = Users[i];
                } catch (KeyNotFoundException) {
                    found = 0;
                    break;
                }
            }

            return found;
        }

        public User GetUserIndex(int index) {
            User found = null;

            foreach (KeyValuePair<int, User> itr_usr in Users) {
                if (itr_usr.Key == index) {
                    found = itr_usr.Value;
                    break;
                }
            }

            return found;
        }

        public int GetUserPost(User index) {
            int found = -1;

            foreach (KeyValuePair<int, User> itr_usr in Users) {
                if (itr_usr.Value == index) {
                    found = itr_usr.Key;
                    break;
                }
            }

            return found;
        }

        public void AddUser(User usr) {
            usr.Ready = 1;

            int slot = NearestSlot();
            Users.Add(slot, usr);

            CurrentUser++;
            Event(1, usr);
        }

        public void RemoveUser(User usr) {
            var item = Users.First(itr => itr.Value.Username == usr.Username);
            Users.Remove(item.Key);

            CurrentUser--;
            if (CurrentUser > 0) {
                // Bruh room master is client side
                Event(2, usr, item.Key);
            } else {
                // Indicate that room is abandoned
                RoomManager.Remove(this);
            }
        }

        public int NearestSlot() {
            int result = 0;

            for (int i = 0; i < 10; i++) {
                if (!Users.ContainsKey(i)) {
                    result = i;
                    break;
                }
            }

            return result;
        }

        public int Slot(User usr) {
            foreach (KeyValuePair<int, User> itr in Users) {
                if (usr == itr.Value) {
                    return itr.Key;
                }
            }

            return -1;
        }

        public User[] GetUsers() {
            return Users.Values.ToArray();
        }

        public void Event(int num, User usr = null, int pos = 0, int bg = 0, int ojn = 0) {
            switch(num) {
                case 1: { // Case when someone enter room
                    PacketBuffer buf = new();
                    buf.Write((short)0);
                    buf.Write((short)0x0bbc);
                    buf.Write(usr.Username);
                    buf.Write(usr.Level);
                    buf.Write((byte)usr.Char.Gender);
                    buf.Write((byte)pos);
                    buf.Write(new byte[2]);

                    int[] chars = usr.Char.ToArray();
                    for (int i = 0; i < chars.Length; i++) {
                        buf.Write(chars[i]);
                    }

                    buf.Write(usr.MusicLength);
                    buf.Write(usr.MusicCount);
                    buf.Write(new byte[2]);

                    byte[] array = buf.ToArray();

                    foreach (KeyValuePair<int, User> itr in Users) {
                        itr.Value.Connection.Send(array);
                    }

                    break;
                }

                case 2: { // Case when someone left room
                    int slot = Slot(usr);

                    foreach (KeyValuePair<int, User> itr in Users) {
                        itr.Value.Connection.Send(new byte[] {
                            0x06, 0x00,
                            0xbf, 0x0b,
                            0x00, (byte)slot // which slot that Leave
                        });
                    }

                    break;
                }

                case 3: { // Case when room change bg
                    foreach (KeyValuePair<int, User> itr in Users) {

                    }
                    break;
                }

                case 4: { // Case when room change OJN ID
                    foreach (KeyValuePair<int, User> itr in Users) {

                    }
                    break;
                }

                case 5: { // Case when someone ready or not ready
                    int slot = Slot(usr);

                    foreach (KeyValuePair<int, User> itr in Users) {
                        itr.Value.Connection.Send(new byte[] {
                            0x06, 0x00, 0xa9, 0x0f, 0x00, (byte)slot
                        });
                    }
                    break;
                }
            }
        }
    }
}
