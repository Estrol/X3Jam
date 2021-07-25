// Author: Randam (Alucard#3792) and Estrol (Estrol#0021)
// License: MIT
// Source: (Private Project) Pokedex + X3Jam

using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Estrol.X3Jam.Utility.Parser {
    public class ItemListParser {
        public static ItemList[] LoadData(string filePath) {
            byte[] data = File.ReadAllBytes(filePath);

            using MemoryStream ms = new(data);
            using BinaryReader br = new(ms);

            int count = (int)br.ReadInt16();
            List<ItemList> lists = new(count);

            for (int i = 0; i < count -1; i++) {

                ItemList item = new() {
                    Id = br.ReadInt32(),
                    ItemCategory = br.ReadByte(),
                    Planet = br.ReadByte(),
                    Flags = br.ReadUInt16(), // Gender...
                    Amount = br.ReadUInt16(),
                    ItemSpecial = br.ReadByte(),
                    ItemFunction = (ItemFunction)br.ReadByte(),
                    PayMethod = br.ReadByte(),
                    PriceGold = br.ReadInt32(),
                    PriceEP = br.ReadInt32(),
                    RoomRenderCategory = br.ReadByte(),
                };

                switch (item.Flags) {
                    // Female
                    case 0: {
                        item.Gender = ItemGender.Female;
                        break;
                    }

                    // Male
                    case 128: {
                        item.Gender = ItemGender.Male;
                        break;
                    }

                    // Female and Male
                    case 256: {
                        item.Gender = ItemGender.Both;
                        break;
                    }

                    // Female and New
                    case 2048: {
                        item.Gender = ItemGender.Female;
                        break;
                    }

                    // Male and New
                    case 2176: {
                        item.Gender = ItemGender.Male;
                        break;
                    }

                    // Both and New
                    case 2304: {
                        item.Gender = ItemGender.Both;
                        break;
                    }

                    default: {
                        item.Gender = ItemGender.Unknown;
                        break;
                    }
                }

                if (i == 0) {
                    item.Files = new string[11];
                } else {
                    item.Files = new string[42];
                }

                int nameLen = br.ReadInt32();

                item.Name = Encoding.UTF8.GetString(br.ReadBytes(nameLen));

                int descLen = br.ReadInt32();

                item.Description = Encoding.UTF8.GetString(br.ReadBytes(descLen));

                int nameCount = nameLen + descLen;
                if (nameCount > 0) {
                    for (int i2 = 0; i2 < item.Files.Length; i2++) {
                        if (i != 0) br.ReadByte(); // Padding as randam says /shrug

                        int itemLen = br.ReadInt32();
                        if (itemLen > 0) {
                            item.Files[i2] = Encoding.UTF8.GetString(br.ReadBytes(itemLen));
                        } else {
                            item.Files[i2] = "";
                        }
                    }
                } else {
                    // This implementation is ugly af, but it works
                    // Basically this exist because weird structure in Itemlist_China.dat
                    int positionToFind = i + 1;
                    int position = 0;
                    while (true) {
                        int pos = (int)br.BaseStream.Position;
                        int current = br.ReadInt32();
                        if (current == positionToFind) {
                            position = pos;
                        } else {
                            br.BaseStream.Position = pos + 1;
                        }

                        if (position > 0) {
                            break;
                        }
                    }

                    br.BaseStream.Position = position;
                }

                lists.Add(item);
            }

            return lists.ToArray();
        }
    }

    public class ItemList {
        public int Id;
        public byte ItemCategory;
        public byte Planet;
        public ushort Flags;
        public ItemGender Gender;
        public ushort Amount;
        public byte ItemSpecial;
        public ItemFunction ItemFunction;
        public byte PayMethod;
        public int PriceGold;
        public int PriceEP;
        public byte RoomRenderCategory;

        public string Name;
        public string Description;
        public string[] Files;
    }

    public enum ItemFunction {
        Cosmetic = 0,
        Power = 1,
        Arrange = 2,
        Visibility = 3
    }
}
