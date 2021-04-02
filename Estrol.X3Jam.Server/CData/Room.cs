/**
 * NOTE: this source is absolute ass, my codestyle is very terrible and it took me over 4 months
 * TODO: clean this shit (such code style, etc) after it working properly
 */

using System.Linq;
using System.Collections.Generic;

using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Server.CManager;
using Estrol.X3Jam.Server.CUtility;
using Estrol.X3Jam.Server.CData.RoomEnums;
using Estrol.X3Jam.Server.Utils;

namespace Estrol.X3Jam.Server.CData {
    public class Room {
        public RoomManager RoomManager;
        private Dictionary<int, User> Users;
        private List<RoomUser> Players;
        private int PlayerCount;
        private RoomColor Color;

        public string RoomName;
        public string Password;
        public RoomMode Mode;
        public RoomDifficulty Difficulty;
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
        public int Roomarg2;
        public int SongID;
        public int MinLvl;
        public int MaxLvl;

        public Room(RoomManager rm, int _RoomID, string Name, int arg3, User user, byte flag, string password = "", RoomMode mode = RoomMode.VS) {
            RoomManager = rm;

            RoomName = Name;
            RoomID = _RoomID;

            Users = new(8);
            AddUser(user, GetEmptyColor());

            PasswordFlag = flag;
            Password = password;

            WaitForSong = true;
            Arena = RoomArena.Random;
            Speed = RoomSpeed.Speed10;
            IsPlaying = RoomStatus.Waiting;
            Mode = mode;
            Difficulty = RoomDifficulty.EX;
            MaxUser = 8;
            CurrentUser = 1;
            MinLvl = 0;
            MaxLvl = 0;

            Players = new();
            PlayerCount = 0;

            SongID = arg3; // For DEBUG Purarg1e only
        }

        public void SetSongID(int ID) {
            SongID = ID;
            Event(4, null, ID);
        }

        public bool CheckPassword(string passwd) {
            return passwd == Password;
        }

        public int GetNearestEmpty() {
            int found = 0;

            for (int i = 0; i < 8; i++) {
                try {
                    var itr = Users[i];
                    if (itr == null) {
                        found = i;
                        break;
                    }
                } catch (KeyNotFoundException) {
                    found = i; // This could mean the slot is available
                    break;
                }
            }

            return found;
        }

        public RoomColor GetEmptyColor() {
            if (Color == null) Color = RoomColor.Red;
            var ReservedColor = Color;
            Color = Color.Next();

            return ReservedColor;
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

        public void AddUser(User usr, RoomColor color) {
            usr.Ready = 1;
            usr.Color = color;

            int slot = NearestSlot();
            Users.Add(slot, usr);

            CurrentUser++;
            Event(1, usr, (int)color, slot);
        }

        public void RemoveUser(User usr) {
            var item = Users.First(itr => itr.Value.Username == usr.Username);
            int _slot = Slot(usr);
            Users.Remove(item.Key);

            CurrentUser--;
            if (CurrentUser > 0) {
                if (item.Value.IsRoomMaster) {
                    item.Value.IsRoomMaster = false;

                    int slot = item.Key;
                    var user = NearestUser(item.Value);
                    user.IsRoomMaster = true;
                    Event(7, usr, item.Key);
                } else {
                    Event(2, usr, _slot);
                }
            } else {
                // Indicate that room is abandoned

                Log.Write("Room {0} deleted", RoomID);
                RoomManager.Remove(this);
            }
        }

        public User NearestUser(User usr) {
            foreach (KeyValuePair<int, User> itr_usr in Users) {
                if (itr_usr.Value != usr) {
                    return itr_usr.Value;
                }
            }

            return null;
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

        public void Prepare() {
            Players.Clear();

            foreach (KeyValuePair<int, User> itr in Users) {
                itr.Value.IsFinished = false;
            }
        }

        public bool Check() {
            bool flag = true;

            foreach (KeyValuePair<int, User> itr in Users) {
                if (!itr.Value.IsFinished) {
                    flag = false;
                    break;
                }
            }

            return flag;
        }

        public int Slot(User usr) {
            foreach (KeyValuePair<int, User> itr in Users) {
                if (usr == itr.Value) {
                    return itr.Key;
                }
            }

            return -1;
        }

        public void SubmitScore(User usr, ushort kool, ushort great, ushort bad, ushort miss, ushort maxcombo, ushort jam, ushort pass, int score) {
            Players.Add(new() {
                Kool = kool,
                Great = great,
                Bad = bad,
                Miss = miss,
                MaxCombo = maxcombo,
                JamCombo = jam,
                Score = score,
            });

            usr.Connection.Send(new byte[] {
                0x06, 0x00, 0xb1, 0x0f, 0x00, 0x00
            });

            usr.IsFinished = true;

            PlayerCount++;
            if (Check()) {
                Log.Write($"Room {RoomID} finished playing!");

                Players.Sort((p1, p2) => Slot(p1.User).CompareTo(Slot(p2.User)));

                int itr = 1;
                foreach (RoomUser val in Players) {
                    val.Position = itr;

                    itr++;
                }

                Event(6);
            }
        }

        public short GetGemFromScore(int score, int kool) {
            int baseGem = score > 1000000 ? 5000 : 500;
            float multipler = kool > 500 ? 1.5f : 1.2f;

            return (short)(baseGem * multipler);
        }

        public User[] GetUsers() {
            return Users.Values.ToArray();
        }

        public void _callback(object sender, int event_id, object[] args) {
            switch (event_id) {

                default: {
                    Log.Write("Unknown ");

                    break;
                }
            }
        }

        /// <summary>
        /// TODO: refactor the confusing paramters
        /// </summary>
        /// <param name="num"></param>
        /// <param name="usr"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        public void Event(int num, User usr = null, int arg1 = 0, int arg2 = 0, int arg3 = 0) {
            switch(num) {
                case 1: { // Case when someone enter room

                    Log.Write(usr.Char.Gender.ToString() + " A");
                    PacketBuffer buf = new();
                    buf.Write((short)0);                // Length
                    buf.Write((short)0x0bbc);           // Opcode
                    buf.Write((byte)arg2);              // Slot
                    buf.Write(usr.Username);            // null-terminated username
                    buf.Write(usr.Level);               // Level
                    buf.Write((byte)usr.Char.Gender);   // Gender
                    buf.Write((byte)arg1);              // Color
                    buf.Write(new byte[2]);             // padding??? (Unknown)

                    int[] chars = usr.Char.ToArray();
                    for (int i = 0; i < chars.Length; i++) {
                        buf.Write(chars[i]);
                    }

                    buf.Write(usr.MusicLength);
                    buf.Write(usr.MusicCount);
                    buf.SetLength();

                    //buf.Write(new byte[2]);

                    byte[] array = buf.ToArray();

                    foreach (KeyValuePair<int, User> itr in Users) {
                        itr.Value.Connection.Send(array);
                    }

                    break;
                }

                case 2: { // Case when someone left room
                    PacketBuffer buf = new();
                    buf.Write((short)8);
                    buf.Write((short)0xbbf);
                    buf.Write(arg1);

                    byte[] data = buf.ToArray();
                    foreach (KeyValuePair<int, User> itr in Users) {
                        itr.Value.Connection.Send(data);
                    }

                    break;
                } 

                case 3: { // Case when room change arg2
                    PacketBuffer buf = new();
                    buf.Write((short)0);
                    buf.Write((short)0xfa3);
                    if ((RoomArena)arg2 == RoomArena.Random) {
                        buf.Write(new byte[] { (byte)arg1, 0x00, 0x00, 0x80 });
                    } else {
                        buf.Write(arg1);
                    }

                    buf.SetLength();
                    byte[] data = buf.ToArray();

                    foreach (KeyValuePair<int, User> itr in Users) {
                        itr.Value.Connection.Send(data);
                    }
                    break;
                }

                case 4: { // Case when room change ojn ID
                    using PacketBuffer buf = new();
                    buf.Write((short)8);
                    buf.Write((short)0xfa1);
                    buf.Write(arg1);

                    byte[] data = buf.ToArray();
                    foreach (KeyValuePair<int, User> itr in Users) {
                        itr.Value.Connection.Send(data);
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

                case 6: { // Game Finish
                    using var buf = new PacketBuffer();
                    buf.Write((short)0);
                    buf.Write((short)0xfb2);
                    buf.Write(MaxUser); // Max users

                    for (int i = 0; i < MaxUser; i++) {
                        RoomUser user = Players[i];
                        if (user != null) {
                            buf.Write((byte)Slot(user.User));
                            buf.Write(1);
                            buf.Write(user.Kool);
                            buf.Write(user.Great);
                            buf.Write(user.Bad);
                            buf.Write(user.Miss);
                            buf.Write(user.MaxCombo);
                            buf.Write(user.JamCombo);
                            buf.Write(user.Score);
                            buf.Write(GetGemFromScore(user.Score, user.Kool));
                        } else {
                            buf.Write((byte)i);
                            buf.Write(0);
                        }
                    }

                    buf.SetLength();
                    byte[] data = buf.ToArray();
                    foreach (RoomUser rUser in Players) {
                        rUser.User.Connection.Send(data);
                    }

                    break;
                }

                case 7: { // Case someone left in InGame
                    PacketBuffer buf = new();
                    buf.Write((short)0x08);
                    buf.Write((short)0xbbf);
                    buf.Write(arg1);

                    byte[] data = buf.ToArray();
                    foreach (RoomUser rUser in Players) {
                        rUser.User.Connection.Send(data);
                    }

                    break;
                }

                case 8: { // Case game ping
                    PacketBuffer buf = new();


                    break;
                }
            }
        }
    }
}
