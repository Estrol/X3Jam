﻿using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.Utils;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CCreateRoom: CBase {
        public CCreateRoom(Client client) : base(client) { }

        public override void Code() {
            string Name = DataUtils.GetString(m_client.Message.data);

            int roomID = m_client.Main.RoomManager.EmptyID();
            Room room = new Room(roomID, Name, m_client.UserInfo, 0x0);

            Log.Write("[{0}@{1}] Create a room with name: \"{2}\" at position: {3}, in channel: {4}",
                m_client.UserInfo.Username,
                m_client.IPAddr,
                Name,
                roomID,
                m_client.UserInfo.ChannelID
            );

            m_client.UserInfo.Room = roomID;
            m_client.Main.RoomManager.Add(room);

            Write(new byte[] {
                0x0d, 0x00, 0xd6, 0x07, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00
            });

            Send();
        }
    }
}