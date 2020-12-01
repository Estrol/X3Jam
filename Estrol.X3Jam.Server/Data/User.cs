namespace Estrol.X3Jam.Server.Data {
    public class User {
        private Connection state;

        private string nickname;
        private string username;
        private string password;

        private int ChannelID;
        private int RoomID;
        private int level;

        public User(string[] auth) {
            nickname = auth[0];
            username = auth[0];
            password = auth[1];
            level = 1;
        }

        public string[] GetInfo() {
            return new string[] { username, nickname };
        }

        public void SetConnnection(Connection state) {
            this.state = state;
        }

        public void SendMessage(byte[] data) {
            if (state == null) {
                return;
            }

            state.Send(data);
        }

        public void IncreaseLevel() {
            level++;
        }

        public string GetUsername() {
            return this.username;
        }

        public string GetNickname() {
            return this.nickname;
        }

        public void SetChannel(int? val) {
            if (val != null) {
                this.ChannelID = (int)val;
            } else {
                this.ChannelID = -1;
            }
        }

        public int GetChannel() {
            if (this.ChannelID == null) {
                return -1;
            }

            return (int)this.ChannelID;
        }

        public void SetRoom(int? val) {
            if (val != null) {
                this.RoomID = (int)val;
            } else {
                this.RoomID = -1;
            }
        }

        public int GetRoom() {
            return (int)this.RoomID;
        }
    }
}
