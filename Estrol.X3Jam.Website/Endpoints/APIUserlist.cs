using Estrol.X3Jam.Server;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Website.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Website.Endpoints {
    public class APIUserlist : APIBase {
        public APIUserlist(HTTPClient c, O2JamServer s, WebMain w) : base(c, s, w) { }

        public override void Handle() {
            User[] users = main.Database.GetUsers();
            List<UserInfo> lists = new();

            foreach (User user in users) {
                lists.Add(new UserInfo() {
                    Username = user.Username,
                    Nickname = user.Nickname,
                    Level = user.Level,
                    Wins = user.Char.Wins,
                    Loses = user.Char.Loses,
                    MCash = user.Char.MCash,
                    Gold = user.Char.Gold
                });
            }

            var result = new {
                users = lists.ToArray()
            };

            client.Send(JsonSerializerExtensions.Serialize(result), 200, "application/json");
        }

        public class UserInfo {
            public string Username { set; get; }
            public string Nickname { set; get; }
            public int Level { set; get; }
            public int Wins { set; get; }
            public int Loses { set; get; }
            public int MCash { set; get; }
            public int Gold { set; get; }
        }
    }
}
