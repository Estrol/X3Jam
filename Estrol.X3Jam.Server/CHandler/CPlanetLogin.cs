using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CUtility;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CPlanetLogin: CBase {
        public CPlanetLogin(Client client) : base(client) { }
        public override void Code() {
            string[] UserAuth = DataUtils.GetUserAuthentication(Client.Message.data);

            User usr = Client.Main.Database.Login(UserAuth[0], UserAuth[1]);
            if (usr == null) {
                Write(new byte[] {
                    0x08, 0x00, 0xf0, 0x03, 0xff, 0xff, 0xff, 0xff
                });

                Log.Write("[{0}@{1}] User: {2} attempting to connect in planet login manager with wrong credentinal (could be manually login using custom websocket?).", "null", Client.IPAddr, UserAuth[0]);
            } else {
                var result = Client.Main.Server.GetClient(Client, usr.Username);
                if (result != null) {
                    Write(new byte[] {
                        0x08, 0x00, 0xe9, 0x03, 0xfe, 0xff, 0xff, 0xff
                    });

                    Log.Write("[{0}@{1}] Attempt to connect already connected account!", Client.IPAddr, UserAuth[0]);
                    Send();
                    return;
                }

                Write(new byte[] {
                    0x08, 0x00, 0xe9, 0x03, 0x00, 0x00, 0x00, 0x00
                });

                Log.Write("[{0}@{1}] Resuming session.", UserAuth[0], Client.IPAddr);
            }

            usr.Connection = Client;
            Client.m_user = usr;

            Send();
        }
    }
}
