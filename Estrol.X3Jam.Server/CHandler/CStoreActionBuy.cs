using Estrol.X3Jam.Server.CData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CHandler {
    public class CStoreActionBuy : CBase {
        public CStoreActionBuy(Client client) : base(client) { }

        public override void Code() {
            Send(new byte[] {
                0x4, 0x00, 0x98, 0x13
            });
        }
    }
}
