using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility.Data;
using Estrol.X3Jam.Utility.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CStoreActionBack : CBase {
        public CStoreActionBack(Client client) : base(client) { }

        public override void Code() {
            Item[] items = Main.Database.GetInventory(Client.UserInfo.Username);
            List<Item> effectItems = new();

            Write((short)0);
            Write((ushort)0x1389);
            Write(Client.UserInfo.Char.Gold);
            Write(Client.UserInfo.Char.MCash);
            Write(0); // padding?

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

            Write(effectItems.Count);
            foreach (Item item in effectItems.ToArray()) {
                Write(item.ItemId);
                Write(item.ItemCount);
            }

            SetL();
            Send();
        }
    }
}
