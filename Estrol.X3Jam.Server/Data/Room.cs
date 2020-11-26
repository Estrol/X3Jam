using System;
using System.Collections.Generic;
using System.Linq;
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

            Users = itr.ToArray();
        }

        public void RemoveUser(User usr) {
            List<User> itr = new List<User>(Users);
            itr.Remove(usr);

            Users = itr.ToArray();
        }

        public User[] GetUsers() {
            return Users;
        }
    }
}
