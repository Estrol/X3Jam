using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Utility.Data;
using Estrol.X3Jam.Utility.Parser;
using System;
using System.Collections.Generic;

namespace Estrol.X3Jam.Server.CHandler {
    public class CCharacter: CBase {
        public CCharacter(Client client) : base(client) {}

        public override void Code() {
            Character character = Client.UserInfo.Char;

            Write((short)0);
            Write((short)0x7d1);
            Write(0);
            Write(Client.UserInfo.Nickname);
            Write((byte)character.Gender); // Gender
            Write(10);
            Write(character.MCash); // mCash
            Write(0);
            Write(character.Level);
            Write(0);
            Write(0);
            Write(0);
            Write(0);
            Write(0);
            Write((byte)0);

            // Character
            Write(character.Instrument); // 0
            Write(character.Hair); // 1
            Write(character.Accessory); // 2
            Write(character.Glove); // 3
            Write(character.Necklace); // 4
            Write(character.Cloth); // 5
            Write(character.Pant); // 6
            Write(character.Glass); // 7
            Write(character.Earring); // 8
            Write(character.ClothAccessory); // 9
            Write(character.Shoe); // 10
            Write(character.Face); // 11
            Write(character.Wing); // 12
            Write(character.InstrumentAccessory); // 13
            Write(character.Pet); // 14
            Write(character.HairAccessory); // 15

            List<Item> effectItems = new();

            // Inventory
            // NOTE: I don't know why it must be 30 inventory slot.
            for (int i = 0; i < 30; i++) {
                var item = Client.UserInfo.Inv.GetItemFromIndex(i);

                if (item == null) {
                    Write(0);
                } else {
                    Write(item.ItemId);

                    if (item.Function != ItemFunction.Cosmetic) {
                        effectItems.Add(item);
                    }
                }
            }

            // 5x4 Padding for what?
            for (int i = 0; i < 5; i++) {
                Write(0);
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
