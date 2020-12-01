using System.Net.Sockets;
using Estrol.X3Jam.Server.Data;

namespace Estrol.X3Jam.Server.Data {
    public class Connection {
        public const int MAX_BUFFER_SIZE = 10248;
        public Server m_server;
        public Socket m_socket;
        public User m_user;

        public byte[] m_raw;
        public byte[] m_data;
        public ushort m_length;

        public byte[] Buffer => m_data;
        public ushort Length => m_length;
        public User UserInfo => m_user;

        public void Read() {
            m_server.ReadAgain(this);
        }

        public void Send(short length) {
            m_server.Send(this, Buffer, length);
        }

        public void Send(byte[] data) {
            m_server.Send(this, data);
        }

        public void Send(byte[] data, short length) {
            m_server.Send(this, data, length);
        }
    }
}
