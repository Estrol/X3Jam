using System;

namespace Estrol.X3Jam.Website.Services {
    public static class HTTPStatus {
        public static string GetResponseStatus(int code) {
            if (!Enum.IsDefined(typeof(HTTPResponse), code)) {
                throw new InvalidOperationException("Unknwon HTTPResponse's enum value " + code);
            }

            HTTPResponse enumN = (HTTPResponse)code;
            string name;

            switch (enumN) {
                case HTTPResponse.SwitchingProtocols: {
                    name = "Switching Protocol";
                    break;
                }

                case HTTPResponse.EarlyHints: {
                    name = "Early Hints";
                    break;
                }

                case HTTPResponse.NonAuthoritativeInformation: {
                    name = "Non-Authoritative Information";
                    break;
                }

                case HTTPResponse.NoContent: {
                    name = "No Content";
                    break;
                }

                case HTTPResponse.ResetContent: {
                    name = "Reset Content";
                    break;
                }

                case HTTPResponse.PartialContent: {
                    name = "Partial Content";
                    break;
                }

                case HTTPResponse.MultipleChoices: {
                    name = "Multiple Choice";
                    break;
                }

                case HTTPResponse.MovedPermanently: {
                    name = "Moved Permanently";
                    break;
                }

                case HTTPResponse.SeeOther: {
                    name = "See Other";
                    break;
                }

                case HTTPResponse.TemporaryRedirect: {
                    name = "Temporary Redirect";
                    break;
                }

                case HTTPResponse.PermanentRedirect: {
                    name = "Permanent Redirect";
                    break;
                }

                case HTTPResponse.BadRequest: {
                    name = "Bad Request";
                    break;
                }

                case HTTPResponse.PaymentRequired: {
                    name = "Payment Required";
                    break;
                }

                case HTTPResponse.NotFound: {
                    name = "Not Found";
                    break;
                }

                case HTTPResponse.MethodNotAllowed: {
                    name = "Method Not Allowed";
                    break;
                }

                case HTTPResponse.NotAcceptable: {
                    name = "Not Acceptable";
                    break;
                }

                case HTTPResponse.ProxyAuthenticationRequired: {
                    name = "Proxy Authentication Required";
                    break;
                }

                case HTTPResponse.InternalServerError: {
                    name = "Internal Server Error";
                    break;
                }

                case HTTPResponse.NotImplemented: {
                    name = "Not Implemented";
                    break;
                }

                case HTTPResponse.BadGateway: {
                    name = "Bad Gateway";
                    break;
                }

                case HTTPResponse.ServiceUnavailable: {
                    name = "Service Unavaliable";
                    break;
                }

                case HTTPResponse.GatewayTimeout: {
                    name = "Gateway Timeout";
                    break;
                }

                default: {
                    name = Enum.GetName(typeof(HTTPResponse), code);
                    break;
                }
            }

            return string.Format("{0} {1}", code, name);
        }
    }

    public enum HTTPResponse {
        // Informal
        Continue = 100,
        SwitchingProtocols = 101,
        EarlyHints = 103,

        // Success
        OK = 200,
        Created = 201,
        Accepted = 202,
        NonAuthoritativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,

        // Redirects
        MultipleChoices = 300,
        Ambiguous = 300,
        MovedPermanently = 301,
        Moved = 301,
        Found = 302,
        Redirect = 302,
        SeeOther = 303,
        RedirectMethod = 303,
        NotModified = 304,
        UseProxy = 305,
        Unused = 306,
        TemporaryRedirect = 307,
        RedirectKeepVerb = 307,
        PermanentRedirect = 308,

        // Client Errors
        BadRequest = 400,
        Unauthorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        RequestEntityTooLarge = 413,
        RequestUriTooLong = 414,
        UnsupportedMediaType = 415,
        RequestedRangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        UpgradeRequired = 426,

        // Server Erros
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HttpVersionNotSupported = 505,
    }
}
