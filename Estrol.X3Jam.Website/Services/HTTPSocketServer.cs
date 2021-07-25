using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Website.Services {
    public class HTTPSocketServer {
        private Socket m_ServerSocket;
        private int m_ServerPort;

        public delegate void ServerEventSender(object sender, HTTPClient state);
        public event ServerEventSender OnServerMessage;

        public HTTPSocketServer(int ServerPort) {
            m_ServerPort = ServerPort;

            m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            m_ServerSocket.Bind(new IPEndPoint(IPAddress.Any, m_ServerPort));
        }

        public void Start() {
            m_ServerSocket.Listen(m_ServerPort);
            m_ServerSocket.BeginAccept(Server_OnAsyncConnection, m_ServerSocket);
        }

        public void Send(HTTPClient state, byte[] data, int length = 0) {
            try {
                state.ClientSocket.BeginSend(data, 0, length, 0, Server_OnAsyncSend, state.ClientSocket);
            } catch (Exception e) {
                HandleException(e, state);
            }
        }

        private void Server_OnAsyncSend(IAsyncResult ar) {
            try {
                Socket socket = (Socket)ar.AsyncState;
                socket.EndSend(ar);

                socket.Disconnect(true);
            } catch (Exception e) {
                HandleException(e, null);
            }
        }

        private void Server_OnAsyncConnection(IAsyncResult ar) {
            HTTPClient state = null;

            try {

                Socket _socket = (Socket)ar.AsyncState;
                state = new() {
                    Main = this,
                    ClientSocket = _socket.EndAccept(ar),
                    ResponseData = new byte[HTTPClient.MAX_BUFFER_SIZE]
                };

                state.ClientSocket.BeginReceive(state.ResponseData, 0, HTTPClient.MAX_BUFFER_SIZE, SocketFlags.None, Server_OnAsyncData, state);
                m_ServerSocket.BeginAccept(Server_OnAsyncConnection, m_ServerSocket);
            } catch (Exception e) {
                HandleException(e, state);
            }
        }

        private void Server_OnAsyncData(IAsyncResult ar) {
            HTTPClient state = null;
            try {
                state = (HTTPClient)ar.AsyncState; 

                state.DataLength = state.ClientSocket.EndReceive(ar);

                byte[] data = new byte[state.DataLength];
                Buffer.BlockCopy(state.ResponseData, 0, data, 0, state.DataLength);

                string stringData = Encoding.ASCII.GetString(data);
                if (stringData.Length == 0) {
                    state.ClientSocket.Disconnect(true);
                    return;
                }

                string[] HeadersData = stringData.Split(new string[] { "\r\n", "\0" }, StringSplitOptions.RemoveEmptyEntries);
                string[] InitialData = HeadersData[0].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                if (InitialData[0] == "\0") {
                    state.ClientSocket.Disconnect(true);
                    return;
                }

                state.Headers = new() {
                    Method = (HTTPMethod)Enum.Parse(typeof(HTTPMethod), InitialData[0], true),
                    URLParams = InitialData[1],
                    HTTPVersion = InitialData[2]
                };

                state.Headers.Host = null;

                for (int i = 1; i < HeadersData.Length; i++) {
                    string[] hData = HeadersData[i].Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (hData.Length == 1) break;

                    string rData = hData[1].Trim();
                    string which = hData[0].Trim();
                    switch (which) {
                        case "Host": {
                            state.Headers.Host = rData;

                            string uriData = rData;
                            if (!uriData.Contains("http")) {
                                uriData = "http://" + uriData;
                            }

                            state.Headers.URLFull = new Uri(uriData + state.Headers.URLParams);
                            break;
                        }

                        case "Connection": {
                            state.Headers.Connection = rData;
                            break;
                        }

                        case "DNT": {
                            if (rData == "null") {
                                state.Headers.DNT = 0;
                            } else {
                                state.Headers.DNT = int.Parse(rData);
                            }

                            break;
                        }

                        case "Upgrade-Insecure-Requests": {
                            state.Headers.UIR = int.Parse(rData);
                            break;
                        }

                        case "User-Agent": {
                            state.Headers.UserAgent = rData;
                            break;
                        }

                        case "Accept": {
                            state.Headers.Accept = rData;
                            break;
                        }

                        case "Content-Type": {
                            state.Headers.ContentType = rData;
                            break;
                        }

                        case "Content-Length": {
                            if (int.TryParse(rData, out int len)) {
                                state.Headers.ContentLength = len;
                            } else {
                                state.Headers.ContentLength = 0;
                            }
                            break;
                        }

                        case "Authorization": {
                            state.Headers.Authorization = rData;
                            break;
                        }

                        case "X3JAM-Post-Data": {
                            state.Headers.X3JAMPostData = rData;
                            break;
                        }
                    }
                }

                if (state.Headers.Method == HTTPMethod.OPTIONS) {
                    HandleCorsMessage(state);
                    return;
                }

                if (state.Headers.Method == HTTPMethod.POST) {
                    state.BodyArray = Encoding.ASCII.GetBytes(HeadersData[^1].Replace("\0", ""));
                }

                if (state.Headers.ContentType != null && state.Headers.ContentType.Contains("application/x-x3jam-base64-encoded-data")) {
                    if (state.Headers.X3JAMPostData != null) {
                        byte[] binaryData = Convert.FromBase64String(state.Headers.X3JAMPostData);
                        state.Headers.X3JAMPostData = Encoding.ASCII.GetString(binaryData);
                    } else {
                        state.Headers.X3JAMPostData = "";
                    }
                }

                if (OnServerMessage == null) return;
                OnServerMessage(this, state);
            } catch (Exception e) {
                if (e.Message.Contains("Requested value")) {
                    string ResponseString = "HTTP/1.1 400 Bad Request\r\n";
                    ResponseString += "Server: Estrol's dotnet HTTPSocketServer for X3-JAM\r\n";
                    ResponseString += "Content-Length: 0\r\n";
                    ResponseString += "\r\n";
                    ResponseString += "Invalid HTTP method";

                    byte[] data = Encoding.ASCII.GetBytes(ResponseString);
                    Send(state, data, data.Length);
                    return;
                }

                HandleException(e, state);
            }
        }

        private void HandleCorsMessage(HTTPClient client) {
            string ResponseString = "HTTP/1.1 204 No Content\r\n";
            ResponseString += "Server: Estrol's dotnet HTTPSocketServer for X3-JAM\r\n";
            ResponseString += $"Access-Control-Allow-Origin: *\r\n";
            ResponseString += "Access-Control-Allow-Methods: GET,HEAD,POST\r\n";
            ResponseString += "Access-Control-Allow-Headers: Content-Type, Accept, Authorization, X3JAM-Post-Data\r\n";
            ResponseString += "Access-Control-Max-Age: 68400\r\n";
            ResponseString += "Vary: Access-Control-Request-Headers\r\n";
            ResponseString += "Content-Length: 0\r\n";

            byte[] data = Encoding.ASCII.GetBytes(ResponseString);
            Send(client, data, data.Length);
        }

        private void HandleException(Exception e, HTTPClient client) {
            if (e is ObjectDisposedException) {
                Console.WriteLine("[C# Website Exception] A thread tried to access disposed object.");
            } else if (e is SocketException err) {
                if (err.ErrorCode == 10054) {
                    Console.WriteLine("[C# Website Exception] A thread tried to access socket that already disconnected");
                }
            } else {
                Console.WriteLine("[C# Website Unhandled Exception] {0}\n{1}", e.Message, e.StackTrace);
            }
        }
    }
}
