using System;
using System.Net;
using System.Net.Sockets;
using Estrol.X3Jam.Server.Data;

namespace Estrol.X3Jam.Server {
    public class Server {
        private ServerMain Main;
        private Socket ServerSocket;
        private short gamePort;
        private short webPort;

        public delegate void ServerEventSender(object sender, Connection state);
        public event ServerEventSender OnServerMessage;

        public Server(ServerMain Main, short gamePort, short webPort) {
            this.Main = Main;
            this.gamePort = gamePort;
            this.webPort = webPort;

            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, this.gamePort));
        }

        public void Start() {
            ServerSocket.Listen(gamePort);
            Console.WriteLine("[Server] Server now listening at port {0}", gamePort);

            ServerSocket.BeginAccept(Server_OnAsyncConnection, ServerSocket);
        }

        public void Send(Connection state, byte[] data, short length = 0) {
            if (length == 0) {
                length = BitConverter.ToInt16(data, 0);
            }

            try {
                state.Socket.BeginSend(data, 0, length, 0, Server_OnAsyncSend, state);
            } catch (Exception e) {
                HandleException(e);
            }
        }

        public void ReadAgain(Connection state) {
            state.raw = new byte[Connection.MAX_BUFFER_SIZE];
            state.Buffer = null;
            state.Socket.BeginReceive(state.raw, 0, Connection.MAX_BUFFER_SIZE, SocketFlags.None, Server_OnAsyncData, state);
        }

        private void Server_OnAsyncSend(IAsyncResult ar) {
            try {
                //Connection state = (Connection)ar.AsyncState;
                //state.raw = new byte[Connection.MAX_BUFFER_SIZE];
                //state.Buffer = null;
                //state.Socket.BeginReceive(state.raw, 0, Connection.MAX_BUFFER_SIZE, SocketFlags.None, Server_OnAsyncData, state);
            } catch (Exception e) {
                HandleException(e);
            }
        }

        private void Server_OnAsyncConnection(IAsyncResult ar) {
            Connection state;

            try {
                Socket _socket = (Socket)ar.AsyncState;

                state = new Connection() {
                    Socket = _socket.EndAccept(ar),
                    Server = this,
                    raw = new byte[Connection.MAX_BUFFER_SIZE]
                };

                state.Socket.BeginReceive(state.raw, 0, Connection.MAX_BUFFER_SIZE, SocketFlags.None, Server_OnAsyncData, state);
                ServerSocket.BeginAccept(Server_OnAsyncConnection, ServerSocket);
            } catch (Exception e) {
                HandleException(e);
            }
        }

        private void Server_OnAsyncData(IAsyncResult ar) {
            try {
                Connection state = (Connection)ar.AsyncState;
                 
                state._opcode = BitConverter.ToUInt16(state.raw, 2);
                state.opcode = (Packets)state._opcode;

                state.Length = BitConverter.ToInt16(state.raw, 0);
                state.Buffer = new byte[state.Length];
                Buffer.BlockCopy(state.raw, 0, state.Buffer, 0, state.Length);
                state.raw = null;

                if (OnServerMessage == null) return;
                OnServerMessage(this, state);
            } catch (Exception e) {
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
                Console.WriteLine("[C# Unhandled Exception] {0}", e.Message);
            }
        }
    }
}
