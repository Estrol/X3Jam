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
        public int Level { set; get; }
        public byte[] MusicCount { set; get; }
        public int MusicLength { set; get; }
        public int ChannelID { set; get; }
        public Client Connection { set; get; }
        public int Room { set; get; }
        public Character Char { set; get; }

        public User(string[] auth, Character character) {
            Nickname = auth[0];
            Username = auth[0];
            Level = 1;

            Char = character;
        }

        public void Message(byte[] data) {
            if (Connection == null) {
                return;
            }

            Connection.Send(data);
        }
    }

    public class Character {
        public string Username { set; get; }
        public string Nickname { set; get; }
        public int Rank { set; get; }
        public int Level { set; get; }
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

        public int[] ToArray() => new[] {
            Instrument, Hair, Accessory, Glove,
            Necklace, Cloth, Pant, Glass,
            Earring, ClothAccessory, Shoe, Face,
            Wing, InstrumentAccessory, Pet, HairAccessory
        };
    }
}
