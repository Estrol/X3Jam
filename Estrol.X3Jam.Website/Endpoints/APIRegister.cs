using System;
using System.Text.Json;
using Estrol.X3Jam.Server;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Website.Services;

namespace Estrol.X3Jam.Website.Endpoints {
    public class APIRegister : APIBase {

        public APIRegister(HTTPClient c, O2JamServer m, WebMain w) : base(c, m, w) { }

        public override void Handle() {
            if (client.Headers.Method != HTTPMethod.POST) {
                var result = new {
                    status = 403,
                    message = "Method not allowed in this endpoint"
                };

                client.Send(JsonSerializer.Serialize(result), 403, "application/json");
                return;
            }

            try {
                var data = JsonSerializerExtensions.DeserializeAnonymousType(client.BodyString.Replace("\0", ""), new { username = "", email = "", nickname = "", password = "", invite_code = "" });

                var acc = main.Database.Exists(data.username, data.email);
                if (acc.IsExist) {
                    var result = new {
                        success = false,
                        message = acc.Reason
                    };

                    client.Send(JsonSerializer.Serialize(result), 409, "application/json");
                } else {
                    try {
                        var IsValidCode = main.Database.VerifyInviteCode(data.invite_code);
                        if (IsValidCode) {
                            main.Database.Register(data.username, data.nickname, data.password, data.email);
                            var result = new {
                                success = true,
                                message = "User successfully registered, you can login using this account now!"
                            };

                            client.Send(JsonSerializer.Serialize(result), 200, "application/json");
                        } else {
                            var result = new {
                                success = false,
                                message = "Invalid invite code, might not exist or already used!"
                            };

                            client.Send(JsonSerializer.Serialize(result), 200, "application/json");
                        }
                    } catch (Exception e) {
                        var result = new {
                            success = false,
                            message = e.Message
                        };

                        client.Send(JsonSerializer.Serialize(result), 200, "application/json");
                    }
                }
            } catch (Exception e) {
                Log.Write(e.Message);

                var result = new {
                    success = false,
                    message = "Internal Server Error: " + e.Message
                };

                client.Send(JsonSerializer.Serialize(result), 200, "application/json");
            }
        }
    }
}
