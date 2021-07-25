/// WARNING: You're about to see garbage like spagheti code of mine
/// prepare to get something you don't usually see in .NET about HTTP server
/// - Estrol#0021

using System;
using System.IO;
using System.Web;
using System.Text;
using System.Linq;
using System.Text.Json;
using System.Collections.Generic;

using Estrol.X3Jam.Server;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility.Parser;
using Estrol.X3Jam.Website.Services;
using System.Xml;
using Estrol.X3Jam.Website.Types;
using Estrol.X3Jam.Website.Endpoints;
using Estrol.X3Jam.Website.Pages;

namespace Estrol.X3Jam.Website {
    public class WebMain {
        public HTTPSocketServer listener;
        public Configuration config;
        public O2JamServer main;
        public int port;

        public bool loop = false;
        public string GitHash;

        public string[] PrivatePath = {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf"),
            AppDomain.CurrentDomain.BaseDirectory
        };

        private Dictionary<string, Type> endpoints;
        private Dictionary<string, Type> dynamicPages;

        public WebMain(O2JamServer main, int port, string githash) {
            this.main = main;
            this.port = port;

            endpoints = new();
            dynamicPages = new();

            config = new();
            GitHash = githash.Replace("\n", "");
            listener = new(port);
            //listener.OnServerMessage += HandleRequest;
            listener.OnServerMessage += OnClientRequest;
        }

        public void Initalize() {
            listener.Start();

            endpoints.Add("userlist", typeof(APIUserlist));
            endpoints.Add("version", typeof(APIVersion));
            endpoints.Add("register", typeof(APIRegister));
            endpoints.Add("musiclist", typeof(APIMusiclist));
            endpoints.Add("generate_invite_code", typeof(APIInviteCode));

            dynamicPages.Add("/Patch/a.html", typeof(PlanetNews));
            dynamicPages.Add("/Patch/b.txt", typeof(ChatAnnouncement));
            dynamicPages.Add("/userlist.aspx", typeof(Userlist));
            dynamicPages.Add("/gamefind/gamefine_main.asp", typeof(Userlist));
            dynamicPages.Add("/gamefind/gamefine_main.aspx", typeof(Userlist));

            Log.Write("O2-JAM RESTful API now listening at port {0}", this.port);
        }

        public void OnClientRequest(object obj, HTTPClient client) {
            string BaseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "www");

            string[] navigation = client.Headers.URLFull.AbsolutePath.Split("/", StringSplitOptions.RemoveEmptyEntries);
            if (navigation.Length == 0) {
                if (client.Headers.URLFull.AbsolutePath == "/") {
                    if (File.Exists(Path.Combine(BaseDirectory, "index.html"))) {
                        string data = File.ReadAllText(Path.Combine(BaseDirectory, "index.html"));
                        client.Send(data, 200, "text/html");
                    } else {
                        client.Send("<?xml version=\"1.0\" encoding=\"utf-8\" ?><errors xmlns=\"http://schemas.google.com/g/2005\"><error><reason>404 - Not found</reason></error><code>404</code></errors>",
                            404,
                            "text/xml"
                        );
                    }
                }

                return;
            }

            switch (navigation[0].ToLower()) {
                case "api": {
                    HandleAPIRequest(client, navigation);
                    break;
                }

                default: {
                    bool IsPageExist = dynamicPages.TryGetValue(client.Headers.URLFull.AbsolutePath, out var PageType);
                    if (IsPageExist) {
                        PageBase page = (PageBase)Activator.CreateInstance(PageType, new object[] { client, main, this });
                        page.Handle();
                        break;
                    }

                    string path = BaseDirectory;

                    foreach (string str in navigation) {
                        path = Path.Combine(path, str);
                    }

                    if (!IsValidPath(path, BaseDirectory)) break;

                    if (File.Exists(path)) {
                        byte[] fileData = File.ReadAllBytes(path);
                        string contentType = "text/plain";

                        if (path.EndsWith(".css")) {
                            contentType = "text/css";
                        } else if (path.EndsWith(".html")) {
                            contentType = "text/html";
                        } else if (path.EndsWith(".js")) {
                            contentType = "text/javascript";
                        } else if (path.EndsWith(".json")) {
                            contentType = "application/json";
                        } else if (path.EndsWith(".php")) {
                            contentType = "text/html";
                        } else if (path.EndsWith(".asp")) {
                            contentType = "text/html";
                        } else if (path.EndsWith(".aspx")) {
                            contentType = "text/html";
                        }

                        string fileText = Encoding.ASCII.GetString(fileData);
                        client.Send(fileText, 200, contentType);
                        break;
                    }

                    if (File.Exists(Path.Combine(BaseDirectory, "404.html"))) {
                        string data = File.ReadAllText(Path.Combine(BaseDirectory, "404.html"));
                        client.Send(data, 404, "text/html");
                        break;
                    }

                    client.Send("<?xml version=\"1.0\" encoding=\"utf-8\" ?><errors xmlns=\"http://schemas.google.com/g/2005\"><error><reason>404 - Not found</reason></error><code>404</code></errors>",
                        404,
                        "text/xml"
                    );
                    break;
                }
            }
        }

        private bool IsValidPath(string path1, string path2) {
            string fullRoot = Path.GetFullPath(path2);
            string pathToVerify = Path.GetFullPath(path1);

            return pathToVerify.StartsWith(fullRoot);
        }

        private void HandleAPIRequest(HTTPClient client, string[] navigation) {
            string version;
            if (navigation.Length == 1) {
                version = "null";
            } else {
                version = navigation[1].ToLower();
            }

            switch (version) {
                case "v1": {
                    string endpoint;
                    if (navigation.Length == 2) {
                        endpoint = "default";
                    } else {
                        endpoint = navigation[2].ToLower();
                    }

                    bool res = endpoints.TryGetValue(endpoint, out var APIName);
                    if (res) {
                        APIBase api = (APIBase)Activator.CreateInstance(APIName, new object[] { client, main, this });
                        api.Handle();
                    } else {
                        var result = new {
                            status = 404,
                            message = "Invalid API endpoint, try check the documentation?"
                        };

                        client.Send(JsonSerializer.Serialize(result), 404, "application/json");
                    }

                    break;
                }

                default: {
                    var result = new {
                        status = 404,
                        message = "Invalid API version, try check the documentation?"
                    };

                    client.Send(JsonSerializer.Serialize(result), 404, "application/json");
                    break;
                }
            }
        }

        public void HandleRequest(object o, HTTPClient wc) {
            switch (wc.Headers.URLFull.AbsolutePath) {

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

                case "/payment/payment_input.asp": {
                    if (wc.Headers.Method != HTTPMethod.POST) {
                        wc.Send("Method not allowed", 403, "text/html");
                        break;
                    }

                    if (wc.Headers.ContentType != "application/x-www-form-urlencoded") {
                        wc.Send("ContentType not supported in this path", 403, "text/html");
                        break;
                    }

                    var HTMLFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "www");
                    if (!Directory.Exists(HTMLFolder)) {
                        wc.Send("Error: cannot find www folder, unable to procced the purchase", 404, "text/html");
                        goto default;
                    }

                    var HTMLFile = Path.Combine(HTMLFolder, "payment.html");
                    if (!File.Exists(HTMLFile)) {
                        wc.Send("Error: cannot find payment.html folder, unable to procced the purchase", 404, "text/html");
                        goto default;
                    }

                    string rawFile = File.ReadAllText(HTMLFile);
                    rawFile = rawFile.Replace("X3JAM_PURCHASE_DATA_TO_REPLACE", wc.BodyString);

                    wc.Send(rawFile, 200, "text/html");
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
                        var users = channel.GetUsers();
                        if (users.Length > 0) {
                            totals += $"CH-{channel.m_ChannelID} [{users.Length}]<br/>";
                            foreach (User user in users) {
                                count++;
                                totals += $"- [{user.Level}] {user.Nickname}</br>";
                            }
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
                    if (wc.Headers.Method != HTTPMethod.GET) {
                        goto default;
                    }
                    string IPAddr = wc.Headers.URLFull.Host;
                    StringBuilder sb = new();
                    sb.Append(string.Format("1 127.0.0.1 o2jam/patch {0}:15000 ", IPAddr));
                    for (int i = 0; i < 6; i++)
                        sb.Append(string.Format("{0} {1} ", IPAddr, 15010));

                    wc.Send(sb.ToString());
                    break;
                }

                case "/api/v1/generate_invite_code": {
                    if (wc.Headers.Method != HTTPMethod.POST) {
                        wc.Send("Method not allowed", 403, "text/html");
                        break;
                    }

                    string authorization = $"Bearer {Base64Encode(config.ini.IniReadValue("WEBSITE", "InviteCodeKey"))}";
                    if (wc.Headers.Authorization == null || wc.Headers.Authorization.Contains(authorization) != true) {
                        wc.Send("{\"status\":403,\"reason\":\"Invalid authorization code\"}", 403, "application/json");
                        break;
                    }

                    string key = main.Database.GenerateInviteCode();

                    JSON_GenerateResponse msg = new() {
                        invite_code = key
                    };

                    var res = JsonSerializer.Serialize(msg);

                    wc.Send(res, 200, "application/json");
                    break;
                }

                case "/api/v1/register": {
                    if (wc.Headers.Method != HTTPMethod.POST) {
                        wc.Send("Method not allowed", 403, "text/html");
                        break;
                    }

                    try {
                        var data = JsonSerializer.Deserialize<JSON_RegisterPOST>(wc.BodyString.Replace("\0", ""));

                        var acc = main.Database.Exists(data.username, data.email);
                        if (acc.IsExist) {
                            var msg = new JSON_RegisterResponse() {
                                success = false,
                                message = acc.Reason
                            };

                            var msg_str = JsonSerializer.Serialize(msg);
                            wc.Send(msg_str, 409, "application/json");
                        } else {
                            try {
                                JSON_RegisterResponse msg;

                                var IsValidCode = main.Database.VerifyInviteCode(data.invite_code);
                                if (IsValidCode) {
                                    main.Database.Register(data.username, data.nickname, data.password, data.email);
                                    msg = new() {
                                        success = true,
                                        message = "User successfully registered, you can login using this account now!"
                                    };
                                } else {
                                    msg = new() {
                                        success = false,
                                        message = "Invalid Invite code for that user or invite code not exist for that user!"
                                    };
                                }

                                var msg_str = JsonSerializer.Serialize(msg);
                                wc.Send(msg_str, 200, "application/json");
                            } catch (Exception e) {
                                var msg = new JSON_RegisterResponse() {
                                    success = false,
                                    message = e.Message
                                };

                                var msg_str = JsonSerializer.Serialize(msg);
                                wc.Send(msg_str, 401, "application/json");
                            }
                        }
                    } catch (Exception e) {
                        Log.Write(e.Message);
                        var msg = new JSON_RegisterResponse() {
                            success = false,
                            message = "Internal Server Error: " + e.Message
                        };

                        var msg_str = JsonSerializer.Serialize(msg);
                        wc.Send(msg_str, 500, "application/json");
                    }
                    break;
                }

                case "/api/v1/userinfo": {
                    string username_parameter = wc.Headers.URLFull.Query;
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
                    if (wc.Headers.Method != HTTPMethod.GET) {
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

                case "/api/v1/shop_items": {
                    if (wc.Headers.Method != HTTPMethod.POST) {
                        wc.Send("{\"status\":403, \"message\":\"Method not allowed\"}", 403, "application/json");
                        break;
                    }

                    if (!wc.Headers.ContentType.Contains("application/x-x3jam-base64-encoded-data")) {
                        wc.Send("{\"status\":400, \"message\":\"Invalid HTTP Header\"}", 400, "application/json");
                        break;
                    }

                    Log.Write(wc.Headers.X3JAMPostData);
                    List<ItemList> validItem = new();
                    var parsed = ParsePOST(wc.Headers.X3JAMPostData);
                    foreach (int i in parsed.items) {
                        var Item = Array.Find(main.Database.ItemLists,
                            x => x.Id == i
                        );

                        if (Item != null) {
                            validItem.Add(Item);
                        }
                    }

                    ShopResult result = new();

                    if (validItem.Count == 0) {
                        result.IsSuccess = false;
                        result.Items = Array.Empty<ShopItem>();
                        var msg1 = JsonSerializer.Serialize(result);
                        wc.Send(msg1, 200, "application/json");
                        break;
                    }

                    result.IsSuccess = true;
                    List<ShopItem> lists = new();
                    foreach (ItemList item in validItem) {
                        Log.Write(item.Name);

                        lists.Add(new() {
                            Name = item.Name,
                            Amount = item.Amount
                        });
                    }

                    result.Items = lists.ToArray();
                    var msg = JsonSerializer.Serialize(result);
                    wc.Send(msg, 200, "application/json");

                    break;
                }

                case "/api/v1/shop_purchase": {
                    if (wc.Headers.Method != HTTPMethod.POST) {
                        wc.Send("{\"status\":403, \"message\":\"Method not allowed\"}", 403, "application/json");
                        break;
                    }

                    if (wc.Headers.ContentType != "application/x-x3jam-base64-encoded-data") {
                        wc.Send("{\"status\":400, \"message\":\"Invalid HTTP Header\"}", 400, "application/json");
                        break;
                    }

                    var parsed = ParsePOST(wc.Headers.X3JAMPostData);
                    var loginResult = main.Database.Login(parsed.Username, parsed.Password);
                    if (loginResult == null) {
                        wc.Send("{\"status\":403, \"message\":\"Invalid username or password\"}", 403, "application/json");
                        break;
                    }

                    List<ItemList> validItem = new();
                    foreach (int i in parsed.items) {
                        var Item = Array.Find(main.Database.ItemLists,
                            x => x.Id == i
                        );

                        if (Item != null) {
                            validItem.Add(Item);
                        }
                    }

                    if (validItem.Count == 0) {
                        wc.Send("{\"status\":400, \"message\":\"Invalid item list\"}", 400, "application/json");
                        break;
                    }

                    int success = 0;
                    var inventory = main.Database.GetInventory(parsed.Username);
                    foreach (ItemList item in validItem) {
                        for (int i = 0; i < 30; i++) {
                            var slot = inventory[i];
                            if (slot.ItemId == 0) {
                                success += 1;

                                main.Database.SetInventory(parsed.Username, i, item.Id, item.Amount);
                                continue;
                            }
                        }
                    }

                    PaymentResult res = new() {
                        IsSuccess = true,
                        Message = success == 0 ? "Purchase went wrong, try again later" : $"Purchase success ({success} of {validItem.Count} items are success)"
                    };

                    var msg = JsonSerializer.Serialize(res);
                    wc.Send(msg, 200, "application/json");
                    break;
                }

                case "/api/v1/musiclist": {
                    if (wc.Headers.Method != HTTPMethod.GET) {
                        wc.Send("{\"status\":403, \"message\":\"Method not allowed\"}", 403, "application/json");
                        break;
                    }

                    string channel_parameter = wc.Headers.URLFull.Query;
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
                    Log.Write(wc.Headers.URLFull.AbsoluteUri);
                    wc.Send("404", 404);
                    break;
                }
            }
        }

        private PaymentPOST ParsePOST(string data) {
            List<int> items = new();

            PaymentPOST result = new();

            foreach (string itr in data.Split("&").Where(x => !string.IsNullOrEmpty(x))) {
                string itr_data = itr.Trim();
                itr_data = itr_data.Replace("\n", "");
                itr_data = itr_data.Replace("\r", "");

                string[] data2 = itr_data.Split("=").Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                if (data2[0] == "pwd") {
                    result.Password = data2[1];
                } else if (data2[0] == "gameid") {
                    result.Username = data2[1];
                    continue;
                } else if (data2[0].Contains("aid")) {
                    if (int.TryParse(data2[1], out var res)) {
                        items.Add(res);
                    } else {
                        items.Add(0);
                    }
                } else if (data2[0].Contains("sid")) {
                    if (int.TryParse(data2[1], out var res)) {
                        items.Add(res);
                    } else {
                        items.Add(0);
                    }
                } else if (data2[0].Contains("tid")) {
                    if (int.TryParse(data2[1], out var res)) {
                        items.Add(res);
                    } else {
                        items.Add(0);
                    }
                }
            }

            result.items = items.ToArray();
            return result;
        }

        public static string Base64Encode(string plainText) {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
