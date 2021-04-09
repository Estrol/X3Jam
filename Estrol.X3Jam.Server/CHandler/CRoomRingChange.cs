using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.IO;

namespace Estrol.X3Jam.Server.CHandler {
    public class CRoomRingChange : CBase {
        public CRoomRingChange(Client client) : base(client) { }

        public override void Code() {
            File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + "\\yes.dmg", Client.Message.full_data);
            int totalRing = BitConverter.ToInt32(Client.Message.data, 2);
            if (totalRing > 3) {
                Log.Write("Total ring expected to get max 3 but got more than 3 which {0}", totalRing);
            }

            byte[] ring_data;
            if (totalRing > 0) {
                ring_data = new byte[totalRing * 4];
                Buffer.BlockCopy(Client.Message.data, 6, ring_data, 0, totalRing * 4);
            } else {
                ring_data = new byte[2];
            }

            Room room = RoomManager.GetID(Client.UserInfo.Room);
            room.RingData = ring_data;
            room.RingCount = totalRing;

            Write((short)0);
            Write((short)0xfb8);
            Write((byte)1);
            Write(totalRing);
            Write(ring_data);
            SetL();

            byte[] data = ToArray();
            foreach (User usr in room.GetUsers())
                usr.Connection.Send(data);
        }
    }
}
