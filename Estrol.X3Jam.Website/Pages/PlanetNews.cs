using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Website.Pages {
    public class PlanetNews : PageBase {
        public PlanetNews(HTTPClient c, O2JamServer s, WebMain m) : base (c, s, m) { }

        public override void Handle() {
            StringBuilder str = new();
            str.Append("<!DOCTYPE html>");
            str.Append("<html>");

            str.Append("<body style=\"background-color:black;\">");
            str.Append("<p><span style=\"color: #ffffff\";>");
            str.Append("<b><strong>X3-JAM Server</strong></br></b>");
            str.Append("</span></br>");
            str.Append("<span style=\"color: #ffffff;\">");

            str.Append("Version 1.8-r2</br>");
            str.Append("- Multiplayer now supported</br>");
            str.Append("- Inventory now supported</br>");
            str.Append("- Shop now supported</br>");
            str.Append("- idk else</br>");
            str.Append("</br>");
            str.Append("Thank you for testing this server</br>");
            str.Append("</span></p></body></html>");

            client.Send(str.ToString(), 200, "text/html");
        }
    }
}
