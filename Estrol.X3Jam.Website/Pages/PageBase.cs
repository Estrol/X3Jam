using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website.Services;
using System;

namespace Estrol.X3Jam.Website.Pages {
    public class PageBase {
        public HTTPClient client;
        public O2JamServer main;
        public WebMain web;

        public PageBase(HTTPClient c, O2JamServer m, WebMain w) {
            client = c;
            main = m;
            web = w;
        }

        public virtual void Handle() {
            throw new NotImplementedException();
        }
    }
}
