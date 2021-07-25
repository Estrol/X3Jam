using System;

namespace Estrol.X3Jam.Website.Services {
    public class HTTPHeader {
        public HTTPMethod Method;
        public Uri URLFull;

        public string HTTPVersion;
        public string URLParams;
        public string Host;
        public string UserAgent;
        public string Accept;
        public string ContentType;
        public string Connection;
        public string Authorization;
        public string X3JAMPostData; // Custom Header field
        public int ContentLength;
        public int DNT;
        public int UIR;
    }
}
