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
using System;
using System.IO;

namespace Estrol.X3Jam.Server.CData {
    public class Room {
        public RoomManager RoomManager;
        private Dictionary<int, User> Users;
        private List<RoomUser> Players;
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

        public int CurrentUser => Users.Count;
        public int MaxUser;
        public int RoomID;
        public int Roomarg2;
        public int SongID;
        public int MinLvl;
        public int MaxLvl;
        public int RandomArenaNumber;

        public Room(RoomManager cManager, int cID, string cName, User cUser, byte cFlag, string cPassword = "", RoomMode cMode = RoomMode.VS) {
            RoomManager = cManager;

            // Properties
            RoomName = cName;
            RoomID = cID;
            Password = cPassword;

            // Flags
            Mode = cMode;
            Arena = RoomArena.Random;
            Speed = RoomSpeed.Speed10;
            IsPlaying = RoomStatus.Waiting;
            Difficulty = RoomDifficulty.EX;
            WaitForSong = true;
            PasswordFlag = cFlag;
            MaxUser = 8;
            MinLvl = 0;
            MaxLvl = 0;

            // Room
            Random rand = new();
            RandomArenaNumber = rand.Next(0, 12);

            // Users
            Users = new(8);
            cUser.IsRoomMaster = true;

            AddUser(cUser, GetEmptyColor());
        }

        public void SetSongID(int ID, int Diff, int RSpeed) {
            SongID = ID;
            Difficulty = (RoomDifficulty)Diff;
            Speed = (RoomSpeed)RSpeed;
            Event(4, null, ID, Diff, RSpeed);
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

            //CurrentUser++;
            Event(1, usr, (int)color, slot);
        }

        public void RemoveUser(User usr) {
            var item = Users.First(itr => itr.Value.Username == usr.Username);
            int _slot = Slot(usr);
            Users.Remove(item.Key);

            //CurrentUser--;
            if (CurrentUser > 0) {
                if (item.Value.IsRoomMaster) {
                    item.Value.IsRoomMaster = false;

                    int slot = item.Key;
                    var user = NearestUser(item.Value);
                    user.IsRoomMaster = true;
                }

                Event(2, usr, _slot);
            } else {
                // Indicate that room is abandoned

                Log.Write("(ch {0}, room: {1}) Deleted", RoomManager.Channel.m_ChannelID, RoomID);
                RoomManager.Remove(this);
            }
        }

        public int GetRoomMaster() {
            foreach (KeyValuePair<int, User> itr in Users) {
                if (itr.Value.IsRoomMaster) {
                    return Slot(itr.Value);
                }
            }

            return 0;
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

            for (int i = 0; i < 8; i++) {
                if (!Users.ContainsKey(i)) {
                    result = i;
                    break;
                }
            }

            return result;
        }

        public void GamePrepare() {
            Players = new(MaxUser);
            IsPlaying = RoomStatus.Playing;
            // Mode = RoomMode.JAM;

            Log.Write("(ch {0}, room: {1}) Now playing song: {2}", RoomManager.Channel.m_ChannelID, RoomID, RoomManager.Channel.GetSongName(SongID));
            RoomManager.Send(3, this, null, 2);
            foreach (KeyValuePair<int, User> itr in Users) {
                itr.Value.IsFinished = false;
                Players.Add(new() {
                    Slot = Slot(itr.Value),
                    User = itr.Value
                });
            }
        }

        public bool GameCheck() {
            bool flag = true;

            foreach (KeyValuePair<int, User> itr in Users) {
                if (!itr.Value.IsFinished) {
                    flag = false;
                    break;
                }
            }

            return flag;
        }

        public bool IsReady() {
            bool flag = true;

            foreach (KeyValuePair<int, User> itr in Users) {
                if (itr.Value.Ready == 0) {
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
            var pUser = Players.First(item => item.User == usr);
            pUser.Kool = kool;
            pUser.Great = great;
            pUser.Bad = bad;
            pUser.MaxCombo = maxcombo;
            pUser.JamCombo = jam;
            pUser.Score = score;
            usr.IsFinished = true;

            if (GameCheck()) {
                IsPlaying = RoomStatus.Waiting;
                // Mode = RoomMode.VS;

                // TODO: Clean up
                Log.Write("(ch {0}, room: {1}) Finish Playing!", RoomManager.Channel.m_ChannelID, RoomID);
                List<RoomUser> plrs = new List<RoomUser>(Players)
                    .OrderBy(prop => prop.Score).ToList();

                for (int i = 0; i < 8; i++) {
                    if (plrs.ElementAtOrDefault(i) != null) {
                        RoomUser roomUser = plrs[i];
                        RoomUser player = Players.Find(x => x.Slot == roomUser.Slot);
                        player.Position = i;
                    }
                }

                RoomManager.Send(3, this, null, 1);
                RoomManager.Send(4, this);
                Event(6);
            }
        }

        public byte[] Submit(User usr, int score) {
            var pUser = Players.First(item => item.User == usr);
            pUser.Score = score;

            byte[] scores = new byte[] {
                0xff, 0xff, 0xff, 0xff,
                0xff, 0xff, 0xff, 0xff
            };

            if (CurrentUser > 1) {
                bool IsEmpty = true;
                foreach (RoomUser user in Players) {
                    if (user.Score != 0) {
                        IsEmpty = false;
                    }
                }

                if (!IsEmpty) {
                    List<RoomUser> plrs = new List<RoomUser>(Players)
                        .OrderByDescending(prop => prop.Score).ToList();

                    for (int i = 0; i < 8; i++) {
                        if (plrs.ElementAtOrDefault(i) != null) {
                            RoomUser roomUser = plrs[i];
                            scores[roomUser.Slot] = (byte)i;
                        }
                    }
                }
            }

            return scores;
        }

        public User[] GetUsers() {
            return Users.Values.ToArray();
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

                    byte[] array = buf.ToArray();

                    foreach (KeyValuePair<int, User> itr in Users) {
                        itr.Value.Connection.Send(array);
                    }

                    break;
                }

                case 2: { // Case when someone left room
                    PacketBuffer buf = new();
                    buf.Write((short)6);
                    buf.Write((short)0xbbf);
                    buf.Write((byte)arg1);
                    buf.Write((byte)GetRoomMaster());

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
                    buf.Write((short)arg1); // OJN ID
                    buf.Write((byte)arg2); // Difficulty
                    buf.Write((byte)arg3); // Speed

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

                    var IsAlone = Players.Count == 1;

                    for (int i = 0; i < MaxUser; i++) {
                        if (Players.ElementAtOrDefault(i) != null) {
                            RoomUser uPlayer = Players[i];
                            buf.Write((byte)i);
                            buf.Write(1);
                            buf.Write(uPlayer.Kool);
                            buf.Write(uPlayer.Great);
                            buf.Write(uPlayer.Bad);
                            buf.Write(uPlayer.Miss);
                            buf.Write(uPlayer.MaxCombo);
                            buf.Write(uPlayer.JamCombo);
                            buf.Write(uPlayer.Score);
                            buf.Write(GetGemFromScore(uPlayer.Score, uPlayer.Kool));
                            buf.Write(uPlayer.User.Level);
                            buf.Write(0);
                            buf.Write((short)(IsAlone ? 1 : uPlayer.Position));

                            //Log.Write($"[DEBUG] User: {uPlayer.User.Username} {uPlayer.Kool} {uPlayer.Great} {uPlayer.Bad} {uPlayer.Miss} {uPlayer.MaxCombo} {uPlayer.JamCombo} {uPlayer.Score} {GetGemFromScore(uPlayer.Score, uPlayer.Kool)}");
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
            }
        }

        private static short GetGemFromScore(int score, int kool) {
            int baseGem = score > 1000000 ? 5000 : 500;
            float multipler = kool > 500 ? 1.5f : 1.2f;

            return (short)(baseGem * multipler);
        }
    }
}
