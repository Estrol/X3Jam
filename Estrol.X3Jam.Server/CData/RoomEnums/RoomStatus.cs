using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CData {
    public enum RoomStatus : byte {
        Waiting = 0x01,
        Playing = 0x02
    }
}
