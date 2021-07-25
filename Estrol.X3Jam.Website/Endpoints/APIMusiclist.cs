using Estrol.X3Jam.Server;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Website.Services;
using Estrol.X3Jam.Website.Types;
using System.Collections.Generic;
using System.Text.Json;
using System.Web;

namespace Estrol.X3Jam.Website.Endpoints {
    public class APIMusiclist : APIBase {
        public APIMusiclist(HTTPClient c, O2JamServer m, WebMain w) : base(c, m, w) { }

        public override void Handle() {
            if (client.Headers.Method != HTTPMethod.GET) {
                client.Send("{\"status\":403, \"message\":\"Method not allowed\"}", 403, "application/json");
                return;
            }

            string channel_parameter = client.Headers.URLFull.Query;
            if (string.IsNullOrWhiteSpace(channel_parameter)) {
                client.Send("{\"status\":400,\"reason\":\"Empty parameter channel!\"}", 400, "application/json");
                return;
            }

            string ch_string = HttpUtility.ParseQueryString(channel_parameter).Get("channel");
            if (string.IsNullOrWhiteSpace(ch_string)) {
                client.Send("{\"status\":400,\"reason\":\"Empty parameter channel!\"}", 400, "application/json");
                return;
            }

            bool res = int.TryParse(ch_string, out var id);
            if (res == null) {
                client.Send("{\"status\":400,\"reason\":\"Invalid int string!\"}", 404, "application/json");
                return;
            }

            Channel channel = main.ChannelManager.GetChannelByID(id);
            if (channel == null) {
                client.Send("{\"status\":404,\"reason\":\"Cannot find the specific channel!\"}", 404, "application/json");
                return;
            }

            List<JSON_OJN> lists = new();

            foreach (OJN ojn in channel.GetMusicList()) {
                lists.Add(new() {
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

            var result = new {
                list = lists.ToArray()
            };

            client.Send(JsonSerializer.Serialize(result), 200, "application/json");
        }
    }
}
