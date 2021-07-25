using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Utility.ClientData.Enums;
using Estrol.X3Jam.Utility.Data;
using Estrol.X3Jam.Utility.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CInventoryEquip : CBase {
        public CInventoryEquip(Client client) : base(client) { }

        public override void Code() {
            int CharSlot = BitConverter.ToInt32(Client.Message.Data, 2);
            int InvSlot = BitConverter.ToInt32(Client.Message.Data, 6);

            Item inventory_itr = Client.UserInfo.Inv.GetItemFromIndex(InvSlot);
            var CharacterSlot = (CharacterRenderSlot)CharSlot;

            int InvInt = inventory_itr != null ? inventory_itr.ItemId : 0;
            int CharInt = GetCharacterValue(CharacterSlot);

            SetCharacterValue(CharacterSlot, InvInt);
            Client.UserInfo.Inv.SetItemFromIndex(InvSlot, inventory_itr);

            Main.Database.SetChar(Client.UserInfo.Username, InvInt, CharacterSlot);
            Main.Database.SetInventory(Client.UserInfo.Username, InvSlot, CharInt, 0);

            Write((short)0x18);
            Write((short)0x138d);
            Write(0);
            Write(CharSlot);
            Write(InvInt);
            Write(InvSlot);
            Write(CharInt);

            Send();
        }

        private int GetCharacterValue(CharacterRenderSlot slot) {
            Character character = Client.UserInfo.Char;

            int result = (int)character.GetType()
                .GetProperty(Enum.GetName(typeof(CharacterRenderSlot), slot))
                .GetValue(character, null);

            return result;
        }

        private void SetCharacterValue(CharacterRenderSlot slot, int itemId) {
            Character character = Client.UserInfo.Char;
            character.GetType()
                .GetProperty(Enum.GetName(typeof(CharacterRenderSlot), slot))
                .SetValue(character, itemId);
        }
    }
}
