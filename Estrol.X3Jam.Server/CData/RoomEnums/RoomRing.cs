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
}
