using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Utility.Networking {
    public class TCPServer {
        private Socket m_ServerSocket;
        private short m_gamePort;

        public delegate void ServerEventSender(object sender, Client state);
        public event ServerEventSender OnServerMessage;

        public delegate void ServerErrorSender(object sender, Client state);
        public event ServerErrorSender OnServerError;

        private List<Client> clients;

        public TCPServer(short gamePort) {
            m_gamePort = gamePort;
            clients = new();

            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ServerSocket.Bind(new IPEndPoint(IPAddress.Any, m_gamePort));
        }

        public void Start() {
            m_ServerSocket.Listen(m_gamePort);
            Log.Write("O2-JAM Game Server now listening at port {0}", m_gamePort);

            m_ServerSocket.BeginAccept(Server_OnAsyncConnection, m_ServerSocket);
        }

        public void Send(Client state, byte[] data, short length = 0) {
            if (length == 0) {
                length = BitConverter.ToInt16(data, 0);
            }

            try {
                state.m_socket.BeginSend(data, 0, length, 0, Server_OnAsyncSend, state);
            } catch (Exception e) {
                HandleException(e, state);
            }
        }

        public void ReadAgain(Client state) {
            state.m_raw = new byte[Client.MAX_BUFFER_SIZE];
            state.m_data = null;
            state.m_socket.BeginReceive(state.m_raw, 0, Client.MAX_BUFFER_SIZE, SocketFlags.None, Server_OnAsyncData, state);
        }

        private void Server_OnAsyncSend(IAsyncResult ar) {}

        private void Server_OnAsyncConnection(IAsyncResult ar) {
            Client state = null;

            try {
                Socket _socket = (Socket)ar.AsyncState;

                state = new Client() {
                    m_socket = _socket.EndAccept(ar),
                    m_server = this,
                    m_raw = new byte[Client.MAX_BUFFER_SIZE]
                };

                clients.Add(state);

                state.m_socket.BeginReceive(state.m_raw, 0, Client.MAX_BUFFER_SIZE, SocketFlags.None, Server_OnAsyncData, state);
                m_ServerSocket.BeginAccept(Server_OnAsyncConnection, m_ServerSocket);
            } catch (Exception e) {
                HandleException(e, state);
            }
        }

        public void RemoveClient(Client cl) {
            clients.Remove(cl);
        }

        public Client GetClient(Client cl, string username) {
            var endpoint = (IPEndPoint)cl.m_socket.RemoteEndPoint;

            foreach (Client client in clients) {
                if (client.UserInfo != null) {
                    var endpoint2 = (IPEndPoint)client.m_socket.RemoteEndPoint;
                    if (endpoint.Address == endpoint2.Address)
                        if (endpoint.Port == endpoint2.Port)
                            continue;

                    if (client.m_user.Username == username)
                        return client;
                }
            }

            return null;
        }

        private void Server_OnAsyncData(IAsyncResult ar) {
            Client state = null;
            try {
                state = (Client)ar.AsyncState; 

                state.m_length = (ushort)state.m_socket.EndReceive(ar);
                state.m_data = new byte[state.Length];
                Buffer.BlockCopy(state.m_raw, 0, state.Buffer, 0, state.Length);
                state.m_raw = null;

                if (OnServerMessage == null) return;
                OnServerMessage(this, state);
            } catch (Exception e) {
                HandleException(e, state);
            }
        }

        private void HandleException(Exception e, Client client) {
            if (e is ObjectDisposedException) {
                Console.WriteLine("[C# Exception] A thread tried to access disposed object.");
            } else if (e is SocketException err) {
                if (err.ErrorCode == 10054) {
                    Console.WriteLine("[C# Exception] A thread tried to access socket that already disconnected");
                    OnServerError?.Invoke(this, client);
                }
            } else {
                Console.WriteLine("[C# Unhandled Exception] {0}\n{1}", e.Message, e.StackTrace);
                if (client != null) {
                    OnServerError?.Invoke(this, client);
                    Console.WriteLine("[Socket] Connection {0} has been forcedly disconnect due to Unhandled Exception", client.IPAddr);
                }
            }
        }
    }
}
