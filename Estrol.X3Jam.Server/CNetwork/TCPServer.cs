using System;
using System.Net;
using System.Net.Sockets;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Server.CNetwork {
    public class TCPServer {
        private Socket m_ServerSocket;
        private short m_gamePort;

        public delegate void ServerEventSender(object sender, Client state);
        public event ServerEventSender OnServerMessage;

        public TCPServer(short gamePort) {
            m_gamePort = gamePort;

            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ServerSocket.Bind(new IPEndPoint(IPAddress.Any, m_gamePort));
        }

        public void Start() {
            m_ServerSocket.Listen(m_gamePort);
            Log.Write("Server now listening for connections in port: {0}", m_gamePort);

            m_ServerSocket.BeginAccept(Server_OnAsyncConnection, m_ServerSocket);
        }

        public void Send(Client state, byte[] data, short length = 0) {
            if (length == 0) {
                length = BitConverter.ToInt16(data, 0);
            }

            try {
                state.m_socket.BeginSend(data, 0, length, 0, Server_OnAsyncSend, state);
            } catch (Exception e) {
                HandleException(e);
            }
        }

        public void ReadAgain(Client state) {
            state.m_raw = new byte[Client.MAX_BUFFER_SIZE];
            state.m_data = null;
            state.m_socket.BeginReceive(state.m_raw, 0, Client.MAX_BUFFER_SIZE, SocketFlags.None, Server_OnAsyncData, state);
        }

        private void Server_OnAsyncSend(IAsyncResult ar) {}

        private void Server_OnAsyncConnection(IAsyncResult ar) {
            Client state;

            try {
                Socket _socket = (Socket)ar.AsyncState;

                state = new Client() {
                    m_socket = _socket.EndAccept(ar),
                    m_server = this,
                    m_raw = new byte[Client.MAX_BUFFER_SIZE]
                };

                state.m_socket.BeginReceive(state.m_raw, 0, Client.MAX_BUFFER_SIZE, SocketFlags.None, Server_OnAsyncData, state);
                m_ServerSocket.BeginAccept(Server_OnAsyncConnection, m_ServerSocket);
            } catch (Exception e) {
                HandleException(e);
            }
        }

        private void Server_OnAsyncData(IAsyncResult ar) {
            Client state = (Client)ar.AsyncState;

            state.m_length = BitConverter.ToUInt16(state.m_raw, 0);
            state.m_data = new byte[state.Length];
            Buffer.BlockCopy(state.m_raw, 0, state.Buffer, 0, state.Length);
            state.m_raw = null;

            if (OnServerMessage == null) return;
            OnServerMessage(this, state);

            try {
                
            } catch (Exception e) {
#if DEBUG
                throw;
#endif
                HandleException(e);
            }
        }

        private void HandleException(Exception e) {
            if (e is ObjectDisposedException) {
                Console.WriteLine("[C# Exception] A thread tried to access disposed object.");
            } else if (e is SocketException) {
                SocketException err = (SocketException)e;
                if (err.ErrorCode == 10054) {
                    Console.WriteLine("[C# Exception] A thread tried to access socket that already disconnected");
                }
            } else {
                Console.WriteLine("[C# Unhandled Exception] {0}\n{0}", e.Message, e.StackTrace);
            }
        }
    }
}
