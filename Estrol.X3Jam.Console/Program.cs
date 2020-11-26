using System;
using Estrol.X3Jam.Server;

namespace Estrol.X3Jam.Console {
    public class Program {
        static void Main(string[] args) {
            System.Console.Title = "X3Jam - O2-JAM 1.8 Server Emulation";

            System.Console.WriteLine("[Message] Welcome to X3-JAM. written by DMJam Dev Group (Estrol and MatVeiQaaa)\n"
                + string.Format("[Message] Current time is {0}", DateTime.Now));

            ServerMain sm = new ServerMain();
            sm.Intialize();

            while (true) {
                System.Console.ReadKey();
            }
        }
    }
}
