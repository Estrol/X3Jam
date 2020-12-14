using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.Data {
    public class Room {
        private User[] Users;
        private User RoomMaster;

        public string RoomName;
        public int RoomID;
        public int RoomBG;
        public byte PasswordFlag;
        public string Password;
        public int SongID;

        public int MaxUser;
        public int CurrentUser;

        public Room(int RoomID, string Name, User user, byte flag, string password = "") {
            this.RoomID = RoomID;
            this.RoomName = Name;
            this.RoomMaster = user;
            this.PasswordFlag = flag;
            this.Password = password;
            this.Users = new User[] {
                user
            };

            this.MaxUser = 8;
            this.CurrentUser = 1;
        }

        public void SetSongID(int ID) {
            this.SongID = ID;
        }

        public void AddUser(User usr) {
            List<User> itr = new List<User>(Users) {
                usr
            };
            CurrentUser++;

            SendByteMessage(1);
            Users = itr.ToArray();
        }

        public void RemoveUser(User usr) {
            List<User> itr = new List<User>(Users);
            itr.Remove(usr);
            CurrentUser--;

            if (usr.Username == RoomMaster.Username && CurrentUser > 1) {
                SetNearestAsRoomMaster();
            }
            SendByteMessage(2);

            Users = itr.ToArray();
        }

        public void SetNearestAsRoomMaster() {

            SendByteMessage(3);
        }

        public User[] GetUsers() {
            return Users;
        }

        public void SendByteMessage(int num) {
            switch(num) {
                case 1: {
                    var ns = new MemoryStream();
                    using (var bw = new BinaryWriter(ns)) {
                        bw.Write((short)0);
                        bw.Write((short)0x0bbc); // Player join room opcode


                    }

                    for (int i = 0; i < Users.Length; i++) {

                    }

                    break;
                }

                case 2: {
                    break;
                }

                case 3: {
                    break;
                }
            }
        }
    }
}
