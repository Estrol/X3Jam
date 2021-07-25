using Estrol.X3Jam.Server;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Website.Services;
using System.Text;

namespace Estrol.X3Jam.Website.Pages {
    public class Userlist : PageBase {
        public Userlist(HTTPClient c, O2JamServer m, WebMain w) : base(c, m, w) { }

        public override void Handle() {
            StringBuilder str = new();
            str.Append("<!DOCTYPE html>");
            str.Append("<html>");

            str.Append("<body style=\"background-color:black;\">");
            str.Append("<p><span style=\"color: #ffffff\";>");
            str.Append("<b><strong>X3-JAM Server</strong></b>");
            str.Append("</span></br>");
            str.Append("<span style=\"color: #ffffff;\">");

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

            str.Append($"Online Users: {count} </br>");
            str.Append(totals);
            str.Append("<button type=\"button\" onClick=\"window.location.reload()\">Refresh</button></span></p></body></html>");

            client.Send(str.ToString(), 200, "text/html");
        }
    }
}
