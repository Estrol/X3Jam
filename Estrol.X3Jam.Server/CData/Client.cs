using Estrol.X3Jam.Server.CNetwork;
using Estrol.X3Jam.Server.CUtility;
using System.Net;
using System.Net.Sockets;

namespace Estrol.X3Jam.Server.CData {
    public class Client {
        public const int MAX_BUFFER_SIZE = 8192;
        public TCPServer m_server;
        public Socket m_socket;
        public User m_user;

        public byte[] m_raw;
        public byte[] m_data;
        public ushort m_length;

        public CMessage Message { set; get; }
        public O2JamServer Main { set; get; }
        public byte[] Buffer => m_data;
        public ushort Length => m_length;
        public User UserInfo => m_user;
        public string IPAddr {
            get {
                IPEndPoint ip = m_socket.RemoteEndPoint as IPEndPoint;

                return ip.Address.ToString();
            }
        }

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
