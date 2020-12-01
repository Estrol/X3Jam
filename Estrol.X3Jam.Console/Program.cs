using System;
using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website;
using System.Diagnostics;

namespace Estrol.X3Jam.Console {
    public class Program {
        static void Main(string[] args) {
            System.Console.Title = "X3Jam - O2-JAM 1.8 Server Emulation";

            System.Console.WriteLine("[Message] Welcome to X3-JAM. written by DMJam Dev Group (Estrol and MatVeiQaaa)\n"
                + string.Format("[Message] Current time is {0}", DateTime.Now));

            ServerMain sm = new ServerMain();
            sm.Intialize();

            WebMain wm = new WebMain(15000);
            wm.Initalize();

            while (true) {
                System.Console.ReadKey();
            }
        }
    }
}
