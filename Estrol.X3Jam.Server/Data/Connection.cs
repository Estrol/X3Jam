using System.Net.Sockets;
using Estrol.X3Jam.Server.Data;

namespace Estrol.X3Jam.Server.Data {
    public class Connection {
        public const int MAX_BUFFER_SIZE = 10248;
        public Server Server;
        public Socket Socket;

        public Packets opcode;
        public ushort _opcode;
        public short Length;
        public byte[] Buffer;
        public byte[] raw;

        public User UserInfo;

        public void Read() {
            Server.ReadAgain(this);
        }

        public void Send(short length) {
            Server.Send(this, Buffer, length);
        }

        public void Send(byte[] data) {
            Server.Send(this, data);
        }

        public void Send(byte[] data, short length) {
            Server.Send(this, data, length);
        }
    }
}
