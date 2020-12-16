using System;
using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website;
using System.Threading;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Console {
    public class Program {
        static void Main(string[] args) {
            System.Console.Title = "X3Jam - O2-JAM 1.8 Server Emulation";

            bool createNew = true;
            using (Mutex mutex = new Mutex(true, "X3JAMSERVER", out createNew)) {
                if (createNew) {
                    System.Console.WriteLine("[Message] Welcome to X3-JAM. written by DMJam Dev Group (Estrol and MatVeiQaaa)\n"
                        + string.Format("[Message] Current time is {0}", DateTime.Now));

                    O2JamServer sm = new O2JamServer();
                    sm.Intialize();

                    WebMain wm = new WebMain(sm, 15000);
                    wm.Initalize();

                    Task NeverReturn = new Task(() => { while (true) { System.Console.Read(); } });
                    NeverReturn.Start();
                    NeverReturn.GetAwaiter().GetResult();
                } else {
                    System.Console.WriteLine("Error, The server program already open!");
                }
            }
        }
    }
}
