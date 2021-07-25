using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Website.Endpoints {
    public class APIInviteCode : APIBase {
        public APIInviteCode(HTTPClient c, O2JamServer m, WebMain w) : base(c, m, w) { }

        public override void Handle() {
            if (client.Headers.Method != HTTPMethod.POST) {
                var res = new {
                    status = 403,
                    message = "Method not allowed in this endpoint"
                };

                client.Send(JsonSerializerExtensions.Serialize(res), 403, "application/json");
                return;
            }

            string authorization = $"Bearer {Base64Encode(web.config.ini.IniReadValue("WEBSITE", "InviteCodeKey"))}";
            if (client.Headers.Authorization == null || client.Headers.Authorization.Contains(authorization) is not true) {
                var res = new {
                    status = 403,
                    message = "Invalid authorization code"
                };

                client.Send(JsonSerializerExtensions.Serialize(res), 403, "application/json");
                return;
            }

            string key = main.Database.GenerateInviteCode();

            var result = new {
                invite_code = key
            };

            client.Send(JsonSerializerExtensions.Serialize(result), 200, "application/json");
        }

        private static string Base64Encode(string plainText) {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
