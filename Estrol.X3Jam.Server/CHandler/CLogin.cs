using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.Utils;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CLogin: CBase {
        public CLogin(Client client) : base(client) { }

        public override void Code() {
            string[] UserAuth = DataUtils.GetUserAuthentication(m_client.Message.data);

            User usr = m_client.Main.Database.Login(UserAuth[0], UserAuth[1]);
            if (usr == null) {
                Write(new byte[] {
                    0x08, 0x00, 0xf0, 0x03, 0xff, 0xff, 0xff, 0xff
                });

                Log.Write("[{0}@{1}] Authenticating for user {2} failed.", "null", m_client.IPAddr, UserAuth[0]);
            } else {
                Write(new byte[] {
                    0x0c, 0x00, 0xf0, 0x03, 0x00, 0x00, 0x00, 0x00,
                    0xa4, 0x9d, 0x00, 0x00
                });

                Log.Write("[{0}@{1}] Authenticating for user {2} success.", "null", m_client.IPAddr, UserAuth[0]);
            }

            Send();
        }
    }
}
