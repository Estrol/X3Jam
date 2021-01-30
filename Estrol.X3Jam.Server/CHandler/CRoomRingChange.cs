using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomRingChange : CBase {
        public CRoomRingChange(Client client) : base(client) { }

        public override void Code() {
            int totalRing = Client.Message.data[3];
            if (totalRing > 3) {
                Log.Write("Total ring expected to get max 3 but got more than 3 which {0}", totalRing);
            }

            byte[] ring_data = new byte[Client.Message.data.Length - 6];
            Room room = RoomManager.GetID(Client.UserInfo.Room);
            room.RingData = ring_data;
            room.RingCount = totalRing;

            Write((short)0);
            Write((short)0xfb8);
            Write((byte)totalRing);
            Write(ring_data);
            SetL();

            Send();
        }
    }
}
