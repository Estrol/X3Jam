using System;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace Estrol.X3Jam.Launcher {
    public class Program {
        public static void Main(string[] args) {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\OTwo.exe")) {
                Console.WriteLine("[+] Cannot find OTwo.exe");
                return;
            }

            Console.WriteLine("[+] Launching Game!");

            IPAddress addr = Dns.GetHostAddresses("ec2-3-135-19-0.us-east-2.compute.amazonaws.com")[0];
            string server = addr.ToString();
            string s_port = "16010";
            string web = addr.ToString();
            string w_port = "16000";

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
