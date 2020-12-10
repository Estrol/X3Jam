namespace Estrol.X3Jam.Server.Data {
    public class User {
        private ClientSocket m_state;

        private string m_nickname;
        private string m_username;

        private int m_channelID;
        private int m_roomID;
        private int m_level;

        public string[] Info => new[] { m_username, m_nickname };
        public string Username => m_username;
        public string Nickname => m_nickname;
        public int Level => m_level;
        public int ChannelID {
            set {
                m_channelID = value;
            }
            get {
                return m_channelID;
            }
        }

        public ClientSocket Connection {
            set {
                m_state = value;
            }
            get {
                return m_state;
            }
        }

        public int Room {
            set {
                m_roomID = value;
            }
            get {
                return m_roomID;
            }
        }

        public void Message(byte[] data) {
            if (m_state == null) {
                return;
            }

            m_state.Send(data);
        }

        public User(string[] auth) {
            m_nickname = auth[0];
            m_username = auth[0];
            m_level = 1;
        }
    }
}
