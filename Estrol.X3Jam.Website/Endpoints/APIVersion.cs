using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website.Services;
using System.Text.Json;

namespace Estrol.X3Jam.Website.Endpoints {
    public class APIVersion : APIBase {
        public APIVersion(HTTPClient c, O2JamServer m, WebMain w) : base(c, m, w) { }

        public override void Handle() {
            var result = new {
                O2JamServerVersion = "1.0.0-r3",
                EstrolHTTPServerVersion = "2.0.1-r1",
                GitVersionHash = ""
            };

            client.Send(JsonSerializer.Serialize(result), 200, "application/json");
        }
    }
}
