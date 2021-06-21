using System.Collections.Generic;
using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Utility.Data {
    public class RoomUser { 
        public int Slot { set; get; }
        public int Position { set; get; }
        public User User { set; get; }
        public RoomColor Color { set; get; }
        public ushort Kool { set; get; }
        public ushort Great { set; get; }
        public ushort Bad { set; get; }
        public ushort Miss { set; get; }
        public ushort JamCombo { set; get; }
        public ushort MaxCombo { set; get; }
        public int Score { set; get; }
        public Dictionary<int, User> QueueExits = new();
    }
}
