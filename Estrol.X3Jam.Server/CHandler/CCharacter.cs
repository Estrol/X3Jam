using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System.Collections.Generic;

namespace Estrol.X3Jam.Server.CHandler {
    public class CCharacter: CBase {
        public CCharacter(Client client) : base(client) {
            PrintTheResult = true;
        }

        public override void Code() {
            Write((short)0);
            Write((short)0x7d1);
            Write(0);
            Write(Client.UserInfo.Nickname);
            Write((byte)Client.UserInfo.Char.Gender); // Female Character
            Write(10);
            Write(6000); // mCash
            Write(0);
            Write(Client.UserInfo.Char.Level);
            Write(0);
            Write(0);
            Write(0);
            Write(0);
            Write(0);
            Write((byte)0);

            int[] Avatar = Client.UserInfo.Char.ToArray();
            for (int i = 0; i < Avatar.Length; i++) {
                Write(Avatar[i]);
            }

            List<RingItem> list = new();

            // Inventory
            // NOTE: I don't know why it must be 35 inventory slot.
            Item[] items = Client.Main.Database.GetInventory(Client.UserInfo.Username);
            for (int i = 0; i < 35; i++) {
                var item = items[i];

                if (item == null) {
                    Write(0);
                } else {
                    Write(item.ItemId);
                    Write(item.ItemCount);

                    if (item.IsRing) {
                        var itr = list.Find((i) => i.Ring == item.RingName);
                        if (itr == null) {
                            list.Add(new() {
                                Ring = item.RingName,
                                Count = item.ItemCount
                            });
                        } else {
                            itr.Count += item.ItemCount;
                        }
                    }
                }
            }

            Write(list.Count);
            foreach (RingItem itr in list.ToArray()) {
                switch (itr.Ring) {
                    case RoomRing.Power: {
                        Write(0x9F);
                        Write(itr.Count);
                        break;
                    }

                    case RoomRing.Mirror: {
                        Write(0x9D);
                        Write(itr.Count);
                        break;
                    }

                    case RoomRing.Random: {
                        Write(0x9B);
                        Write(itr.Count);
                        break;
                    }

                    case RoomRing.Sudden: {
                        Write(0x95);
                        Write(itr.Count);
                        break;
                    }

                    case RoomRing.Dark: {
                        Write(0x93);
                        Write(itr.Count);
                        break;
                    }

                    case RoomRing.Hidden: {
                        Write(0x97);
                        Write(itr.Count);
                        break;
                    }

                    case RoomRing.Panic: {
                        Write(0x99);
                        Write(itr.Count);
                        break;
                    }
                }

                Write(itr.Count);
            }

            // Todo: Analyze this later.
            //Write(new byte[] {
            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x99, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x00, 0x00, 0x00, 0x00, 0x9B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x95, 0x00, 0x00, 0x00, 0x9D, 0x00, 0x00, 0x00, 0x93, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x97, 0x00, 0x00, 0x00, 0x9F, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x12, 0x00, 0x00, 0x00, 0xEF, 0x00, 0x00, 0x01,
            //    0x09, 0x01, 0x1E, 0x01, 0x40, 0x01, 0x4E, 0x01, 0x50, 0x01, 0x51, 0x01, 0x57, 0x01, 0x59, 0x01,
            //    0x84, 0x01, 0x8A, 0x01, 0x93, 0x01, 0xBB, 0x01, 0xDD, 0x01, 0xE1, 0x01, 0xE2, 0x01, 0xE3, 0x01,
            //    0xE4, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            //    0x07, 0x00, 0x00, 0x00, 0x9D, 0x00, 0x00, 0x00, 0xE7, 0x03, 0x00, 0x00, 0x9B, 0x00, 0x00, 0x00,
            //    0xE7, 0x03, 0x00, 0x00, 0x99, 0x00, 0x00, 0x00, 0xE7, 0x03, 0x00, 0x00, 0x97, 0x00, 0x00, 0x00,
            //    0xE7, 0x03, 0x00, 0x00, 0x93, 0x00, 0x00, 0x00, 0xE7, 0x03, 0x00, 0x00, 0x95, 0x00, 0x00, 0x00,
            //    0xE7, 0x03, 0x00, 0x00, 0x9F, 0x00, 0x00, 0x00, 0xE7, 0x03, 0x00, 0x00
            //});

            Log.Write("[{0}@{1}] Get Character and Inventory Info", Client.UserInfo.Username, Client.IPAddr);

            SetL();
            Send();
        }

        public class RingItem {
            public RoomRing Ring;
            public int Count;
        }
    }
}
