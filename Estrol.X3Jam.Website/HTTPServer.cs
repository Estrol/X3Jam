using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Website {
    public class HTTPServer {
        private TcpListener listener;
        private bool loop;

        public delegate void Data(object sender, WebConnection wc);
        public event Data OnDataReceived;

        public HTTPServer(IPAddress ip, int port) {
            listener = new TcpListener(ip, port);
            loop = false;
        }

        public void Start() {
            loop = true;
            listener.Start();
            EventLoop();
        }

        public async Task EventLoop() {
            while (loop) {
                try {
                    TcpClient client = await listener.AcceptTcpClientAsync();

                    ThreadPool.QueueUserWorkItem(HandleClient, client);
                } catch (Exception e) {
                    if (!e.Message.Contains("An existing connection was forcibly closed by the remote host")) {
                        if (e.Message.Contains("Requested value '' was not found."))
                            throw;

                        Console.WriteLine("[Website] Exception: {0}", e.Message);
                    }
                }
            }
        }

        public void HandleClient(object _client) {
            try {
                TcpClient client = (TcpClient)_client;
                NetworkStream ns = client.GetStream();

                WebConnection wc = new WebConnection(client, ns);

                if (wc.forwarded) {
                    OnDataReceived?.Invoke(this, wc);
                }
            } catch (Exception e) {
                if (!e.Message.Contains("An existing connection was forcibly closed by the remote host")) {
                    if (e.Message.Contains("Requested value '' was not found."))
                        throw;

                    Console.WriteLine("[Website] Exception: {0}", e.Message);
                }
            }
        }
    }
}
