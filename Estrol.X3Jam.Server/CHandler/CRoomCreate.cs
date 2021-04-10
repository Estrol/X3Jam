using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CUtility;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CCreateRoom: CBase {
        public CCreateRoom(Client client) : base(client) { }

        public override void Code() {
            string Name = DataUtils.GetString(Client.Message.data);

            byte mode = Client.Message.data[2];
            int roomID = RoomManager.EmptyID();
            Room room = new(RoomManager, roomID, Name, Client.UserInfo, 0x0);
            //Room room = new(RoomManager, roomID, Name, 0x0, Client.UserInfo, 0x0);

            Log.Write("[{0}@{1}] Create a {2} room with name: \"{3}\" at position: {4}, in channel: {5}",
                Client.UserInfo.Username,
                Client.IPAddr,
                mode == 0x1 ? "VS" : "Solo",
                Name,
                roomID,
                Client.UserInfo.ChannelID
            );

            Client.UserInfo.Room = roomID;
            RoomManager.Add(room);

            Write(new byte[] {
                0x0d, 0x00, 0xd6, 0x07, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00
            });

            Send();
        }
    }
}
