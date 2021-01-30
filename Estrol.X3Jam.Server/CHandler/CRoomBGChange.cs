using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomBGChange : CBase {
        public CRoomBGChange(Client client) : base(client) { }

        public override void Code() {
            Client.Message.full_data[2] = 0xa3;

            Room room = RoomManager.GetIndex(Client.UserInfo.Room);
            foreach (User usr in room.GetUsers())
                usr.Connection.Send(Client.Message.full_data);
        }
    }
}
