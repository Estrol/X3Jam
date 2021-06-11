using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CData {
    public enum RoomRing {
        // Power Ring
        Power = 0x9F,

        // Arrange Rings
        Mirror = 0x99,
        Random = 0x9B,
        Panic = 0x9D,

        // Visibility Rings
        Hidden = 0x97,
        Sudden = 0x93,
        Dark = 0x95
    }

    public class ItemIdRings {
        public static int[] Power = { 158, 159 };
        public static int[] Mirror = { 156, 157 };
        public static int[] Random = { 154, 155 };
        public static int[] Panic = { 152, 153 };
        public static int[] Hidden = { 150, 151 };
        public static int[] Sudden = { 148, 149 };
        public static int[] Dark = { 146, 147 };
        public static EnumItemId Get(int ItemId) {
            if (Power.Contains(ItemId)) {
                return new() { Name = "PowerRing", Ring = RoomRing.Power, Id = Power[0] };
            } else if (Mirror.Contains(ItemId)) {
                return new() { Name = "MirrorRing", Ring = RoomRing.Mirror, Id = Mirror[0] };
            } else if (Random.Contains(ItemId)) {
                return new() { Name = "RandomRing", Ring = RoomRing.Random, Id = Random[0] };
            } else if (Panic.Contains(ItemId)) {
                return new() { Name = "PanicRing", Ring = RoomRing.Panic, Id = Panic[0] };
            } else if (Hidden.Contains(ItemId)) {
                return new() { Name = "HiddenRing", Ring = RoomRing.Hidden, Id = Hidden[0] };
            } else if (Sudden.Contains(ItemId)) {
                return new() { Name = "SuddenRing", Ring = RoomRing.Sudden, Id = Sudden[0] };
            } else if (Dark.Contains(ItemId)) {
                return new() { Name = "DarkRing", Ring = RoomRing.Dark, Id = Dark[0] };
            }

            return null;
        }

        public class EnumItemId {
            public string Name;
            public RoomRing Ring;
            public int Id;
        }
    }
}
