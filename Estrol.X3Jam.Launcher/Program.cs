using System;
using System.Diagnostics;

namespace Estrol.X3Jam.Launcher {
    public class Program {
        public static void Main(string[] args) {
            Console.WriteLine("||==|| X3Jam Game Launcher ||==||");
            if (args.Length < 1) {
                Console.WriteLine("[?] launcher.exe <otwo.exe> <server> <s_port> [web] [w_port]");
                return;
            }

            Console.WriteLine("[+] Launching O2-JAM!");

            string server = args[1];
            string s_port = args[2];
            string web  = args[3];
            string w_port = args[4];

            ProcessStartInfo ps = new() {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = args[0],
                Arguments = "1 127.0.0.1 o2jam/patch "
                    + $"{web}:{w_port} "
                    + "1 1 1 1 1 1 1 1 "
                    + $"{server} {s_port} "
                    + $"{server} {s_port} "
                    + $"{server} {s_port} "
                    + $"{server} {s_port} "
                    + $"{server} {s_port} "
                    + $"{server} {s_port} "
                    + $"{server} {s_port} "
                    + $"{server} {s_port} "
            };

            Process proc = Process.Start(ps);
            if (proc == null) {
                Console.WriteLine("[-] Failed to launch!");
                return;
            }

            bool rc = proc.WaitForInputIdle();
            if (!rc) {
                Console.WriteLine("[-] Game crash!");
                return;
            } else {
                rc = Patcher.PatchExecutable(proc);
                if (!rc) {
                    Console.WriteLine("[-] Failed to modify memory!");
                    proc.Kill();
                    return;
                }

                Console.WriteLine("[+] Success patching!");
            }
        }
    }
}
