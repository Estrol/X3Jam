using System;
using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Server.Utils;

namespace Estrol.X3Jam.Server.Handlers {
    public class PlanetLogin : Base {
        public PlanetLogin(Connection state, PacketManager PM, ServerMain _b) : base(state) {
            string[] UserAuth = DataUtils.GetUserAuthentication(PM.data);

            User usr = _b.Database.CredentialsLogin(UserAuth[0], UserAuth[1]);
            if (usr == null) {
                Write(new byte[] {
                    0x08, 0x00, 0xf0, 0x03, 0xff, 0xff, 0xff, 0xff
                });

                Console.WriteLine("[Server] [Error] Username \"{0}\" attempting to login with wrong credential in PlanetLogin (probably bot or database error?)",
                    UserAuth[0]
                );
            } else {
                Write(new byte[] {
                    0x08, 0x00, 0xe9, 0x03, 0x00, 0x00, 0x00, 0x00
                });

                Console.WriteLine("[Server] [{0}] Resuming session!",
                    UserAuth[0]
                );
            }

            state.UserInfo = usr;

            Send();
        }
    }
}
