using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CData.RoomEnums {
    public class RoomUser { 
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
    }
}
