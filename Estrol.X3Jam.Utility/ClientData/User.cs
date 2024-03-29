﻿using Estrol.X3Jam.Utility.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CData {
    public class User {
        public string Username { set; get; }
        public string Nickname { set; get; }
        public string[] Info => new[] { Username, Nickname };
        public int Level => Char.Level;
        public byte[] MusicCount { set; get; }
        public int MusicLength { set; get; }
        public int ChannelID { set; get; } = -1;
        public Client Connection { set; get; }
        public int Room { set; get; } = -1;
        public Character Char { set; get; }
        public Inventory Inv { set; get; }
        public int Ready { set; get; } = 0;
        public bool IsFinished { set; get; } = false;
        public RoomColor Color { set; get; } = 0;
        public bool IsRoomMaster { set; get; } = false;

        public User(string[] auth, Character character, Inventory inventory) {
            Nickname = auth[1];
            Username = auth[0];

            Char = character;
            Inv = inventory;    
        }

        public void Message(byte[] data) {
            if (Connection == null) {
                return;
            }

            Connection.Send(data);
        }
    }

    public class Inventory {
        public Item[] items { set; get; }

        public Item GetItemFromIndex(int index) {
            return items.ElementAtOrDefault(index);
        }

        public void SetItemFromIndex(int index, Item value) {
            if (index > items.Length) {
                return;
            }

            items[index] = value;
        }
    }

    public class Character {
        public string Username { set; get; }
        public string Nickname { set; get; }
        public int Rank { set; get; } = 0;
        public int Level { set; get; } = 0;
        public int MCash { set; get; } = 0;
        public int Gold { set; get; } = 0;
        public int Wins { set; get; } = 0;
        public int Loses { set; get; } = 0;
        public int Scores { set; get; } = 0;
        public int Gender { set; get; } = 0;
        public int Instrument { set; get; } = 0;
        public int Hair { set; get; } = 0;
        public int Accessory { set; get; } = 0;
        public int Glove { set; get; } = 0;
        public int Necklace { set; get; } = 0;
        public int Cloth { set; get; } = 0;
        public int Pant { set; get; } = 0;
        public int Glass { set; get; } = 0;
        public int Earring { set; get; } = 0;
        public int Shoe { set; get; } = 0;
        public int Face { set; get; } = 36;
        public int Wing { set; get; } = 0;
        public int HairAccessory { set; get; } = 0;
        public int InstrumentAccessory { set; get; } = 0;
        public int ClothAccessory { set; get; } = 0;
        public int Pet { set; get; } = 0;

        public int[] ToArray2() => new[] {
            Instrument, Hair, Accessory, Glove,
            Necklace, Cloth, Pant, Glass,
            Earring, Shoe, Pet, Face, Wing,
            InstrumentAccessory, HairAccessory, ClothAccessory
        };

        public int[] ToArray() => new[] {
            Instrument, Hair, Accessory, Glove,
            Necklace, Cloth, Pant, Glass,
            Earring, Shoe, Shoe, Face,
            Wing, InstrumentAccessory, Pet, HairAccessory
        };
    }
}
