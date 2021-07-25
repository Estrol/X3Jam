using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website.Services;

namespace Estrol.X3Jam.Website.Endpoints {
    public abstract class APIBase {
        public HTTPClient client;
        public O2JamServer main;
        public WebMain web;

        public APIBase(HTTPClient c, O2JamServer m, WebMain w) {
            client = c;
            main = m;
            web = w;
        }

        public virtual void Handle() {
            throw new System.NotImplementedException("This Endpoint not yet handled");
        }
    }
}
