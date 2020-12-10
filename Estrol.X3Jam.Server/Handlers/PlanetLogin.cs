using System;
using Estrol.X3Jam.Server.Data;
using Estrol.X3Jam.Server.Utils;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.Handlers {
    public class PlanetLogin : Base {
        public PlanetLogin(ClientSocket state, PacketManager PM, ServerMain _b) : base(state) {
            string[] UserAuth = DataUtils.GetUserAuthentication(PM.data);

            User usr = _b.Database.Login(UserAuth[0], UserAuth[1]);
            if (usr == null) {
                Write(new byte[] {
                    0x08, 0x00, 0xf0, 0x03, 0xff, 0xff, 0xff, 0xff
                });

                Log.Write("[{0}@{1}] User: {2} attempting to connect in planet login manager with wrong credentinal (could be manually login using custom websocket?).", "null", state.IPAddr, UserAuth[0]);
            } else {
                Write(new byte[] {
                    0x08, 0x00, 0xe9, 0x03, 0x00, 0x00, 0x00, 0x00
                });

                Log.Write("[{0}@{1}] Resuming session.", UserAuth[0], state.IPAddr);
            }

            usr.Connection = state;
            state.m_user = usr;

            Send();
        }
    }
}
