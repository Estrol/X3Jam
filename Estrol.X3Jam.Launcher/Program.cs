using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Estrol.X3Jam.Launcher {
    public class Program {
        public static void Main(string[] args) {
            IConfiguration conf = new(AppDomain.CurrentDomain.BaseDirectory + "\\config.ini");

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\OTwo.exe")) {
                Console.WriteLine("[+] Cannot find OTwo.exe");
                return;
            }

            Console.WriteLine("[+] Launching Game!");

            IPAddress addr = Dns.GetHostAddresses(conf.IniReadValue("LAUNCHER", "Domain"))[0];
            string server = addr.ToString();
            string s_port = conf.IniReadValue("LAUNCHER", "ServerPort");
            string web = addr.ToString();
            string w_port = conf.IniReadValue("LAUNCHER", "WebPort");

            ProcessStartInfo ps = new() {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal,
                FileName = "OTwo.exe",
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
            }

            rc = Patcher.PatchExecutable(proc);
            if (!rc) {
                Console.WriteLine("[-] Failed to modify memory!");
                proc.Kill();
                return;
            }
        }
    }
}
