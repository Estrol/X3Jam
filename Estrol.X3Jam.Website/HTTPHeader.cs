using System;

namespace Estrol.X3Jam.Website {
    public class HTTPHeader {
        public HTTPMethod Method;
        public Uri URLFull;

        public string HTTPVersion;
        public string URLParams;
        public string Host;
        public string UserAgent;
        public string Accept;
        public string Connection;
        public int DNT;
        public int UIR;
    }
}
