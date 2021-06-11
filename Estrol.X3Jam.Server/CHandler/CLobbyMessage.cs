using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CUtility;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Estrol.X3Jam.Server.CHandler {
    public class CLobbyMessage: CBase {
        public CLobbyMessage(Client client) : base(client) { }

        public override void Code() {
            string msg = DataUtils.GetString(Client.Message.Data);
            Channel channel = Client.Main.ChannelManager.GetChannelByID(Client.UserInfo.ChannelID);
            User usr = Client.UserInfo;

            if (msg.StartsWith("!") && msg.Length > 1) {
                char[] seperator = { ' ' };
                string[] _args = msg.Replace("!", "").Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                string command = _args[0];
                string[] args = new string[_args.Length - 1];
                Array.Copy(_args, 1, args, 0, _args.Length - 1);

                switch (command) {
                    case "whois": {
                            MessageSend(channel, usr, "Not implemented yet!", true, true);
                        break;
                    }

                    case "annon": {
                        if (args.Length == 0) {
                            MessageSend(channel, usr, "annon <message> [isEveryone=false]", true, true);
                            break;
                        }

                        bool isEveryone = false;
                        string[] bool_ = new[] { "true", "false" };
                        if (bool_.Contains(args[args.Length].ToLower())) {
                            isEveryone = true;
                        }

                        MessageSend(channel, usr, "Sending message", true, !isEveryone);
                        break;
                    }

                    case "count": {
                        MessageSend(channel, usr, string.Format("Current users in CH {0}: {1}", channel.m_ChannelID, channel.GetUsers().Length), true, true);
                        break;
                    }

                    default: {
                        MessageSend(channel, usr, "Unknown commmand!", true, true);
                        break;
                    }
                }
            } else {
                MessageSend(channel, usr, msg);
            }
        }

        private void MessageSend(Channel ch, User usr, string msg, bool isSystem = false, bool isSelf = false) {
            User[] users = ch.GetUsers();

            using var stream = new MemoryStream();
            using var bw = new BinaryWriter(stream); 
            
            bw.Write((short)0); // Initial len header
            bw.Write(new byte[] { 0xdd, 0x07 });

            char[] str;
            if (!isSystem) {
                str = usr.Nickname.ToCharArray();
            } else {
                str = "System".ToCharArray();
            }

            bw.Write(Encoding.UTF8.GetBytes(str));
            bw.Write((byte)0x00);

            char[] c_msg = msg.ToCharArray();
            bw.Write(Encoding.UTF8.GetBytes(c_msg));
            bw.Write((byte)0x00);

            bw.Seek(0, SeekOrigin.Begin);
            bw.Write((short)stream.Length);

            byte[] data = stream.ToArray();

            if (!isSelf) {
                for (int i = 0; i < users.Length; i++) {
                    User user = users[i];

                    user.Message(data);
                }
            } else {
                usr.Message(data);
            }
        }
    }
}
