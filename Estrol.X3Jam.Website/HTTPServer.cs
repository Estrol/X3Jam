using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
                    NetworkStream ns = client.GetStream();

                    WebConnection wc = new WebConnection(client, ns);

                    OnDataReceived?.Invoke(this, wc);
                } catch (Exception e) {
                    if (!e.Message.Contains("An existing connection was forcibly closed by the remote host")) {
                        Console.WriteLine("[Website] Exception: {0}", e.Message);
                    }
                }
            }
        }
    }
}
