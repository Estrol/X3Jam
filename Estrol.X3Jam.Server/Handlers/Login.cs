using System;
using System.Text;
using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Server.Utils;

namespace Estrol.X3Jam.Server.Handlers {
    public class Login : Base {
        public Login(Connection state, PacketManager PM, ServerMain _b) : base(state) {
            string[] UserAuth = DataUtils.GetUserAuthentication(PM.data);

            User usr = _b.Database.CredentialsLogin(UserAuth[0], UserAuth[1]);
            if (usr == null) {
                Write(new byte[] {
                    0x08, 0x00, 0xf0, 0x03, 0xff, 0xff, 0xff, 0xff
                });

                Console.WriteLine("[Server] [Auth] Authenticate for user {0} failed",
                    UserAuth[0]
                );
            } else {
                Write(new byte[] {
                    0x0c, 0x00, 0xf0, 0x03, 0x00, 0x00, 0x00, 0x00,
                    0xa4, 0x9d, 0x00, 0x00
                });

                Console.WriteLine("[Server] [Auth] Authenticate for user {0} success",
                    UserAuth[0]
                );
            }

            Send();
        }
    }
}
