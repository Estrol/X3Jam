using System;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Text.Json;
using System.Diagnostics;
using Estrol.X3Jam.Server;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.Website {
    public class WebMain {
        public HTTPServer listener;
        public O2JamServer main;
        public int port;

        public bool loop = false;

        public WebMain(O2JamServer main, int port) {
            this.main = main;
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

                case "/accounts/register": {
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

                case "/web_api/register": {
                    if (wc.header.Method != HTTPMethod.POST) {
                        wc.Send("<b>Access is denied!</b><br><b1>Unknown method GET!</b1>", 200, "text/html");
                        break;
                    }

                    try {
                        var bytejson = Encoding.UTF8.GetBytes(wc.sBody.Replace("\0", ""));
                        var utf8reader = new Utf8JsonReader(bytejson);
                        var data = JsonSerializer.Deserialize<JSON_RegisterPOST>(ref utf8reader);

                        bool isExist = main.Database.Exists(data.username);
                        if (isExist) {
                            var msg = new JSON_RegisterResponse() {
                                success = false,
                                message = "User already exists"
                            };

                            var msg_str = JsonSerializer.Serialize(msg);
                            wc.Send(msg_str, 409, "application/json");
                        } else {
                            main.Database.Register(data.username, data.password);

                            var msg = new JSON_RegisterResponse() {
                                success = true,
                                message = "User created!"
                            };

                            var msg_str = JsonSerializer.Serialize(msg);
                            wc.Send(msg_str, 200, "application/json");
                        }
                    } catch (Exception e) {
                        Log.Write(e.Message);
                        wc.Send("<b>Internal Server Error</b><br><b1>An error that I won't tell you!</b1>", 500, "text/html");
                    }
                    break;
                }

                default: {
                    wc.Send("404", 404);
                    break;
                }
            }
        }

        public class JSON_RegisterPOST {
            public string username { set; get; }    
            public string password { set; get; }
            public string email { set; get; }
        }

        public class JSON_RegisterResponse {
            public bool success { set; get; }
            public string message { set; get; }
        }
    }
}
