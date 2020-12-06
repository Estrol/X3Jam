using System;
using System.Collections.Generic;
using Estrol.X3Jam.Server.Utils;

namespace Estrol.X3Jam.Server.Data {
    public class ChannelItem {
        private OJNList m_MusicList;
        private User[] m_Users;

        public int m_ChannelID;
        public int m_MaxRoom;
        public int m_CurrentRoom;

        public ChannelItem(int ID, string MusicList, int MaxRoom) {
            m_Users = new User[] { };
            m_CurrentRoom = 0;
            m_ChannelID = ID;
            m_MaxRoom = MaxRoom;
            m_MusicList = OJNListDecoder.Decode(AppDomain.CurrentDomain.BaseDirectory + @"\conf\musiclist\" + MusicList);
        }

        public OJN[] GetMusicList() {
            return m_MusicList.GetHeaders();
        }

        public int GetListCount() {
            return m_MusicList.Count;
        }

        public User[] GetUsers() {
            return m_Users;
        }

        public void AddUser(User usr) {
            List<User> itr = new List<User>(m_Users) {
                usr
            };

            m_Users = itr.ToArray();
        }

        public void RemoveUser(User usr) {
            List<User> itr = new List<User>(m_Users);
            itr.Remove(usr);

            m_Users = itr.ToArray();
        }
    }
}
