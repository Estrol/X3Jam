using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.Data {
    public class ChannelItem {
        private User[] users;
        public int ChannelID;
        public int UsrID;

        public ChannelItem(int ID) {
            this.users = new User[] { };
            this.ChannelID = ID;
            this.UsrID = 0;
        }

        public int GetUsrID() {
            int yeet = UsrID;
            UsrID++;
            return yeet;
        }

        public User[] GetUsers() {
            return users;
        }

        public void AddUser(User usr) {
            List<User> itr = new List<User>(users) {
                usr
            };

            users = itr.ToArray();
        }

        public void RemoveUser(User usr) {
            List<User> itr = new List<User>(users);
            itr.Remove(usr);

            users = itr.ToArray();
        }
    }
}
