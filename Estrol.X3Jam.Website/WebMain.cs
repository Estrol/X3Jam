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
using Estrol.X3Jam.Server.CData;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;

namespace Estrol.X3Jam.Website {
    public class WebMain {
        public HTTPServer listener;
        public O2JamServer main;
        public int port;

        public bool loop = false;
        public string GitHash;

        public WebMain(O2JamServer main, int port, string githash) {
            this.main = main;
            this.port = port;
            GitHash = githash.Replace("\n", "");
            listener = new HTTPServer(IPAddress.Any, port);
            listener.OnDataReceived += HandleRequest;
        }

        public void Initalize() {
            listener.Start();
            Log.Write("O2-JAM RESTful API now listening at port {0}", this.port);
        }

        public void HandleRequest(object o, WebConnection wc) {
            switch (wc.header.URLFull.AbsolutePath) {

                case "/": {
                    wc.Send("<b>Welcome to X3-JAM</b><br><b1>Nothing here for now</b1>", 200, "text/html");
                    break;
                }

                case "/accounts/css/register.css": {
                    var CSSDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "www", "css");

                    if (!Directory.Exists(CSSDirectory)) {
                        goto default;
                    }

                    var CSSFile = Path.Combine(CSSDirectory, "register.css");
                    if (!File.Exists(CSSFile)) {
                        goto default;
                    }

                    string htmlData = File.ReadAllText(CSSFile);
                    wc.Send(htmlData, 200, "text/css");
                    break;
                }

                case "/accounts/register": {
                    var HTMLFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "www");

                    if (!Directory.Exists(HTMLFolder)) {
                        goto default;
                    }

                    var HTMLFile = Path.Combine(HTMLFolder, "register.html");
                    if (!File.Exists(HTMLFile)) {
                        goto default;
                    }

                    string htmlData = File.ReadAllText(HTMLFile);
                    wc.Send(htmlData, 200, "text/html");
                    break;
                }

                case "/userlist.aspx":
                case "/gamefind/gamefine_main.aspx":
                case "/gamefind/gamefine_main.asp": {
                    StringBuilder str = new();
                    str.Append("<!DOCTYPE html><html><body style =\"backgro" +
                        "und-color:black;\"><p><span style=\"color: #ffffff;\"><s" +
                        "trong>X3-JAM Server</strong></span><br/><span s" +
                        "tyle=\"color: #ffffff;\">Online Users:");

                    string totals = "";
                    int count = 0;
                    foreach (Channel channel in main.ChannelManager.channels) {
                        totals += $"CH-{channel.m_ChannelID} [{channel.GetUsers().Length}]<br/>";
                        foreach (User user in channel.GetUsers()) {
                            count++;
                            totals += $"- [{user.Level}] {user.Username}</br>";
                        }

                        totals += "<br/>";
                    }

                    str.Append($" {count}</br>");
                    str.Append(totals);
                    str.Append("<button type=\"button\" onClick=\"window.location.reload()\">Refresh</button></span></p></body></html>");

                    _ = "<!DOCTYPE html><html><body style=\"backgro" +
                        "und-color:black;\"><p><span style=\"color: #ffffff;\"><s" +
                        "trong>Welcome to X3-JAM</strong></span><br/><span s" +
                        "tyle=\"color: #ffffff;\">Online Users:<br/>- None</" +
                        "span></p></body></html>";

                    wc.Send(str.ToString(), 200, "text/html");
                    break;
                }

                case "/patch/b.txt":
                case "/Patch/b.txt": {
                    string NewsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "News.txt");

                    string data = File.ReadAllText(NewsFile);
                    wc.Send(data);
                    break;
                }

                case "/patch/a.html":
                case "/Patch/a.html": {
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

                case "/api/v1/launcherconfig": {
                    if (wc.header.Method != HTTPMethod.GET) {
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

                case "/api/v1/register": {
                    if (wc.header.Method != HTTPMethod.POST) {
                        wc.Send("Method not allowed", 403, "text/html");
                        break;
                    }

                    try {
                        var bytejson = Encoding.UTF8.GetBytes(wc.sBody.Replace("\0", ""));
                        var utf8reader = new Utf8JsonReader(bytejson);
                        var data = JsonSerializer.Deserialize<JSON_RegisterPOST>(ref utf8reader);

                        var acc = main.Database.Exists(data.username, data.email);
                        if (acc.IsExist) {
                            var msg = new JSON_RegisterResponse() {
                                success = false,
                                message = acc.Reason
                            };

                            var msg_str = JsonSerializer.Serialize(msg);
                            wc.Send(msg_str, 409, "application/json");
                        } else {
                            main.Database.Register(data.username, data.password, data.email);

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

                case "/api/v1/userinfo": {
                    string username_parameter = wc.header.URLFull.Query;
                    if (string.IsNullOrWhiteSpace(username_parameter)) {
                        wc.Send("{\"status\":400,\"reason\":\"Empty parameter user!\"}", 400, "application/json");
                        break;
                    }

                    string user_param = HttpUtility.ParseQueryString(username_parameter).Get("user");
                    if (string.IsNullOrWhiteSpace(user_param)) {
                        wc.Send("{\"status\":400,\"reason\":\"Empty parameter user!\"}", 400, "application/json");
                        break;
                    }

                    Character user = main.Database.GetChar(user_param);
                    if (user == null) {
                        wc.Send("{\"status\":404,\"reason\":\"Cannot find the specific user!\"}", 404, "application/json");
                        break;
                    }

                    var info = new JSON_UserInfo() {
                        username = user.Username,
                        nickname = user.Nickname,
                        gender = user.Gender,
                        level = user.Level,
                        mcash = user.MCash,
                        gold = user.Gold,
                        wins = user.Wins,
                        loses = user.Loses,
                        scores = user.Scores
                    };

                    var msg = JsonSerializer.Serialize(info);

                    wc.Send(msg, 200, "application/json");
                    break;
                }

                case "/api/v1/userlist": {
                    if (wc.header.Method != HTTPMethod.GET) {
                        wc.Send("{\"status\":403, \"message\":\"Method not allowed\"}", 403, "application/json");
                        break;
                    }

                    var userlist = new JSON_Userlist();

                    List<JSON_Channel> list = new();
                    foreach (Channel channel in main.ChannelManager.channels) {
                        List<JSON_User> users = new();
                        foreach (User user in channel.GetUsers()) {
                            users.Add(new() {
                                username = user.Username,
                                channel = channel.m_ChannelID,
                                level = user.Level
                            });
                        }

                        list.Add(new() {
                            channel = $"CH{channel.m_ChannelID}",
                            users = users.ToArray(),
                            users_count = users.Count
                        }); 
                    }

                    userlist.channels = list.ToArray();
                    var msg = JsonSerializer.Serialize(userlist);

                    wc.Send(msg, 200, "application/json");
                    break;
                }

                case "/api/v1/musiclist": {
                    if (wc.header.Method != HTTPMethod.GET) {
                        wc.Send("{\"status\":403, \"message\":\"Method not allowed\"}", 403, "application/json");
                        break;
                    }

                    string channel_parameter = wc.header.URLFull.Query;
                    if (string.IsNullOrWhiteSpace(channel_parameter)) {
                        wc.Send("{\"status\":400,\"reason\":\"Empty parameter channel!\"}", 400, "application/json");
                        break;
                    }

                    string ch_string = HttpUtility.ParseQueryString(channel_parameter).Get("channel");
                    if (string.IsNullOrWhiteSpace(ch_string)) {
                        wc.Send("{\"status\":400,\"reason\":\"Empty parameter channel!\"}", 400, "application/json");
                        break;
                    }

                    bool res = int.TryParse(ch_string, out var id);
                    if (res == null) {
                        wc.Send("{\"status\":400,\"reason\":\"Invalid int string!\"}", 404, "application/json");
                        break;
                    }

                    Channel channel = main.ChannelManager.GetChannelByID(id);
                    if (channel == null) {
                        wc.Send("{\"status\":404,\"reason\":\"Cannot find the specific channel!\"}", 404, "application/json");
                        break;
                    }

                    var list = new JSON_MusicList();
                    var listT = new List<JSON_OJN>();

                    foreach (OJN ojn in channel.GetMusicList()) {
                        listT.Add(new() {
                            title = ojn.TitleString,
                            artist = ojn.ArtistString,
                            ojn_name = $"o2ma{ojn.Id}.ojn",
                            bpm = ojn.BPM,
                            id = ojn.Id,
                            level_ex = ojn.LevelEx,
                            level_nx = ojn.LevelNx,
                            level_hx = ojn.LevelHx,
                            note_ex_count = ojn.NoteCountEx,
                            note_nx_count = ojn.NoteCountNx,
                            note_hx_count = ojn.NoteCountHx,
                            duration_ex = ojn.DurationEx,
                            duration_nx = ojn.DurationNx,
                            duration_hx = ojn.DurationHx
                        });
                    }

                    list.list = listT.ToArray();
                    var msg = JsonSerializer.Serialize(list);

                    wc.Send(msg, 200, "application/json");
                    break;
                }

                case "/api/v1/version": {
                    var version = new JSON_Version() {
                        O2JamServerVersion = "1.0.0-r2",
                        EstrolHTTPServerVersion = "1.2.1-r2",
                        GitVersionHash = GitHash
                    };

                    string data = JsonSerializer.Serialize(version);
                    wc.Send(data, 200, "application/json");
                    break;
                }

                default: {
                    wc.Send("404", 404);
                    break;
                }
            }
        }

        public class JSON_Version {
            public string O2JamServerVersion { set; get; }
            public string EstrolHTTPServerVersion { set; get; }
            public string GitVersionHash { set; get; }
        }

        public class JSON_MusicList {
            public JSON_OJN[] list { set; get; }
        }

        public class JSON_OJN {
            public string title { get; set; }
            public string artist { get; set; }
            public string ojn_name { get; set; }
            public float bpm { get; set; }
            public int id { get; set; }
            public int level_ex { get; set; }
            public int level_nx { get; set; }
            public int level_hx { get; set; }
            public int note_ex_count { get; set; }
            public int note_nx_count { get; set; }
            public int note_hx_count { get; set; }
            public int duration_ex { get; set; }
            public int duration_nx { get; set; }
            public int duration_hx { get; set; }
        }

        public class JSON_Userlist {
            public JSON_Channel[] channels { set; get; }
        }

        public class JSON_Channel {
            public string channel { set; get; }
            public int users_count { set; get; }
            public JSON_User[] users { set; get; }
        }

        public class JSON_User {
            public string username { set; get; }
            public int channel { set; get; }
            public int level { set; get; }
        }

        public class JSON_UserInfo {
            public string username { set; get; }
            public string nickname { set; get; }
            public int gender { set; get; }
            public int level { set; get; }
            public int mcash { set; get; }
            public int gold { set; get; }
            public int wins { set; get; }
            public int loses { set; get; }
            public int scores { set; get; }

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
