using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CData {
    public enum RoomMode : byte {
        Solo = 0x0,
        VS = 0x1,
        Unknown = 0x2,
        JAM = 0x3
    }
}
