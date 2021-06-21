using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Utility.Data;
using Estrol.X3Jam.Utility.Parser;
using System.Collections.Generic;
using System.Linq;

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

            List<Item> effectItems = new();

            // Inventory
            // NOTE: I don't know why it must be 35 inventory slot.
            Item[] items = Main.Database.GetInventory(Client.UserInfo.Username);
            for (int i = 0; i < 35; i++) {
                var item = items[i];

                if (item == null) {
                    Write(0);
                } else {
                    Write(item.ItemId);

                    if (item.Function != ItemFunction.Cosmetic) {
                        effectItems.Add(item);
                    }
                }
            }

            // Write all list effect items's Count
            Write(effectItems.Count);

            // Write all list effect items
            foreach (Item item in effectItems.ToArray()) {
                Write(item.ItemId);
                Write(item.ItemCount);
            }

            Log.Write("[{0}@{1}] Get Character and Inventory Info", Client.UserInfo.Username, Client.IPAddr);

            SetL();
            Send();
        }
    }
}
