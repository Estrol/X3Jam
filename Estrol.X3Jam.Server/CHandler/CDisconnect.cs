﻿using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CDisconnect: CBase {
        public CDisconnect(Client client) : base(client) { }

        public override void Code() {
            string usr = "null";

            if (Client.UserInfo != null) {
                Channel ch = Main.ChannelManager.GetChannelByID(Client.UserInfo.ChannelID);
                if (ch != null) {
                    Room room = ch.RManager.GetID(Client.UserInfo.Room);
                    if (room != null) {
                        room.RemoveUser(Client.UserInfo);
                    }

                    ch.RemoveUser(Client.UserInfo);
                }
                usr = Client.UserInfo.Username;
            }

            Log.Write("[{0}@{1}] Disconnected", usr, Client.IPAddr);
            Main.Server.RemoveClient(Client);
            Client.m_socket.Disconnect(true);
        }
    }
}
