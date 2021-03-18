using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.Utils;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CCreateRoom: CBase {
        public CCreateRoom(Client client) : base(client) { }

        public override void Code() {
            string Name = DataUtils.GetString(Client.Message.data);

            int roomID = RoomManager.EmptyID();
            Room room = new(RoomManager, roomID, Name, 0x0, Client.UserInfo, 0x0);

            Log.Write("[{0}@{1}] Create a room with name: \"{2}\" at position: {3}, in channel: {4}",
                Client.UserInfo.Username,
                Client.IPAddr,
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
