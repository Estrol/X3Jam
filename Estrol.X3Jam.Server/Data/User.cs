namespace Estrol.X3Jam.Server.Data {
    public class User {
        private string nickname;
        private string username;
        private string password;
        private int level;

        private int ChannelID;
        private int RoomID;

        public User(string[] auth) {
            nickname = auth[0];
            username = auth[0];
            password = auth[1];
        }

        public string[] GetInfo() {
            return new string[] { username, nickname };
        }

        public string GetUsername() {
            return this.username;
        }

        public void SetChannel(int val) {
            this.ChannelID = val;
        }

        public int GetChannel() {
            return this.ChannelID;
        }

        public void SetRoom(int val) {
            this.RoomID = val;
        }

        public int GetRoom() {
            return this.RoomID;
        }
    }
}
