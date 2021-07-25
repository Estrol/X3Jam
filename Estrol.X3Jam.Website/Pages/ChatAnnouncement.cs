using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website.Services;
using System;
using System.IO;

namespace Estrol.X3Jam.Website.Pages {
    public class ChatAnnouncement : PageBase {
        public ChatAnnouncement(HTTPClient c, O2JamServer s, WebMain m) : base(c, s, m) { }

        public override void Handle() {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "News.txt");
            string file;

            if (File.Exists(filePath) is false) {
                file = "//Lightboard" + Environment.NewLine;
                file += "Todo: Lightboard" + Environment.NewLine;
                file += "//Stateroom" + Environment.NewLine;
                file += "Todo: Stateroom" + Environment.NewLine;
                file += "//StateWaiting" + Environment.NewLine;
                file += "Todo: StateWaiting" + Environment.NewLine;
            } else {
                file = File.ReadAllText(filePath);
            }

            client.Send(file);
        }
    }
}
