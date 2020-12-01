using System;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace Estrol.X3Jam.Website {
    public class WebMain {
        public HTTPServer listener;
        public int port;

        public bool loop = false;

        public WebMain(int port) {
            this.port = port;
            listener = new HTTPServer(IPAddress.Any, port);
            listener.OnDataReceived += HandleRequest;
        }

        public void Initalize() {
            listener.Start();
        }

        public void HandleRequest(object o, WebConnection wc) {
            switch (wc.header.URLParams) {
                case "/": {
                    wc.Send("<b>Welcome to X3-JAM</b><br><b1>Nothing here for now</b1>", 200, "text/html");
                    break;
                }

                case "/gamefind/gamefine_main.asp": {
                    string fine = "<!DOCTYPE html><html><body style=\"backgro" +
                        "und-color:black;\"><p><span style=\"color: #ffffff;\"><s" +
                        "trong>Welcome to X3-JAM</strong></span><br/><span s" +
                        "tyle=\"color: #ffffff;\">Online Users:<br/>- None</" +
                        "span></p></body></html>";

                    wc.Send(fine, 200, "text/html");
                    break;
                }

                case "/patch/b.txt": {
                    string data = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"\conf\news.txt");
                    wc.Send(data);
                    break;
                }

                case "/patch/a.html": {
                    string patch_a = "<!DOCTYPE html><html><body style=\"background" +
                        "-color:black;\"><p><span style=\"color: #ffffff;\"><strong>" +
                        "Welcome to X3-JAM</strong></span><br/><span style=\"color: " +
                        "#ffffff;\">Currently on Alpha ver.</span><br/><br/><span sty" +
                        "le=\"color: #ffffff;\">Multiplayer on progress</span><br/><" +
                        "span style=\"color: #ffffff;\">Shop on progress</span><br/>" +
                        "<br/><span style=\"color: #ffffff;\">Yeah everything is rem" +
                        "a</span><br/><span style=\"color: #ffffff;\">-ked from scra" +
                        "tch.</span><br/><span style=\"color: #ffffff;\">Including s" +
                        "erver files.</span></p></body></html>";

                    wc.Send(patch_a, 200, "text/html");
                    break;
                }

                case "/launcher/config": {
                    if (wc.header.Method != HTTPMethod.POST) {
                        goto default;
                    }
                    string IPAddr = wc.header.URLFull.Host;
                    StringBuilder sb = new StringBuilder();
                    sb.Append(string.Format("1 127.0.0.1 o2jam/patch {0}:15000 ", IPAddr));
                    for (int i = 0; i < 6; i++)
                        sb.Append(string.Format("{0} {1} ", IPAddr, 15010));

                    wc.Send(sb.ToString());
                    break;
                }

                default: {
                    wc.Send("404", 404);
                    break;
                }
            }
        }
    }
}
