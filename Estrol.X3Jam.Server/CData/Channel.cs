using System;
using System.Collections.Generic;
using Estrol.X3Jam.Server.CManager;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CData {
    public class Channel {
        public O2JamServer Main;
        public ChanManager CManager;
        public RoomManager RManager;

        private OJNList m_MusicList;
        private User[] m_Users;

        public int m_ChannelID;
        public int m_MaxRoom;

        public Channel(O2JamServer Main, ChanManager CManager, int ID, string MusicList, int MaxRoom) {
            this.Main = Main;
            this.CManager = CManager;
            this.RManager = new RoomManager(Main, this, MaxRoom);

            m_Users = Array.Empty<User>();
            m_ChannelID = ID;
            m_MaxRoom = MaxRoom;
            m_MusicList = OJNListDecoder.Decode(AppDomain.CurrentDomain.BaseDirectory + @"\conf\musiclist\" + MusicList);
        }

        public int Count => RManager.Count;

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
            List<User> itr = new(m_Users) {
                usr
            };

            m_Users = itr.ToArray();
        }

        public void RemoveUser(User usr) {
            List<User> itr = new(m_Users);
            itr.Remove(usr);

            m_Users = itr.ToArray();
        }

        public string GetSongName(int ID) {
            foreach (OJN ojn in GetMusicList()) {
                if (ojn.Id == ID) {
                    return $"{ojn.TitleString} by {ojn.ArtistString}";
                }
            }

            return "Unknown Song by null";
        }
    }
}
