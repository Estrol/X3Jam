using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CUtility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomMessage: CBase {
        public CRoomMessage(Client client) : base(client) { }

        public override void Code() {
            string msg = DataUtils.GetString(Client.Message.Data);

            byte[] data = DataUtils.CreateMessageA(Client.UserInfo.Nickname, msg);
            Room room = RoomManager.GetID(Client.UserInfo.Room);
            foreach (User usr in room.GetUsers())
                usr.Connection.Send(data);
        }
    }
}
