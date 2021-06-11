using System;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;

using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.IConsole {
    public class Program {
        static ManualResetEvent ExitEvent = new(false);

        static void Main(string[] args) {
            Type t = Type.GetType("Mono.Runtime");
            if (t != null) {
                Console.WriteLine("ERROR: You cannot run this server app from linux or using mono, use linux build instead.");
                return;
            }

            Console.Title = "X3-JAM - Unofficial O2-JAM 1.8 Server";
            PrintLogo();

#if RELEASE
            AppDomain.CurrentDomain.FirstChanceException += (caller, e) => {
                ExceptionHandler(caller, e.Exception);
            };
#endif

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "server.conf");
            string val = "0";
            if (File.Exists(path)) {
                ConfLoader conf = new(path);
                val = conf.IniReadValue("CONFIG", "MultiServer");
            }

            if (int.Parse(val) == 0) {
                using Mutex mutex = new(false, "Global\\{X3JAM-ESTROL-GROUP-SIGNED}", out bool createNew);
                if (createNew) {
                    RunServer();
                } else {
                    if (!IsLinux) {
                        _ = MessageBox(IntPtr.Zero, "The server program already open!\nIf you want enable multiple server please set MultiServer to 1 in config!", "Error", (uint)0x00000010L);
                    } else {
                        Console.WriteLine("ERROR: The server program already open!");
                        Console.WriteLine("ERROR: If you want enable multi server please set MultiServer to 1 in config");
                    }
                }
            } else {
                RunServer();
            }
        }

        private static void RunServer() {
            O2JamServer Server = new();
            Server.Intialize();

            WebMain Website = new(Server, int.Parse(Server.Config.Get("WebPort")), Properties.Resources.version);
            Website.Initalize();

            AppDomain.CurrentDomain.ProcessExit += (caller, e) => {
                Server.Close();
            };

            Console.CancelKeyPress += Console_CancelKeyPress;
            ExitEvent.WaitOne();
        }

#if RELEASE
        private static void ExceptionHandler(object sender, Exception exception) {
            string ReportSTR = "[Report Data]\n";
            ReportSTR += $"Exception: {exception.GetType()}\n";
            ReportSTR += $"Message: {exception.Message}\n";
            ReportSTR += "StackTrace:\n";
            ReportSTR += exception.StackTrace;
            if (exception.InnerException != null) {
                ReportSTR += "\n";
                Exception inner = exception.InnerException;
                ReportSTR += $"InnerException: {inner.GetType()}\n";
                ReportSTR += $"Message: {inner.Message}\n";
                ReportSTR += "InnerStackTrace:\n";
                ReportSTR += inner.StackTrace;
            }

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "crash.txt"), ReportSTR);

            if (!IsLinux) {
                _ = MessageBox(IntPtr.Zero, exception.Message, "X3JAM Runtime Error", (uint)0x00000010L);
                Environment.Exit(-1);
            } else {
                Console.WriteLine("ERROR: An error occurred please report this.");
                Console.WriteLine("ERROR: Report file has been written to /conf/crash.txt");
                Console.WriteLine("ERROR: " + exception.Message);
                Console.WriteLine("ERROR: " + exception.StackTrace);
                Environment.Exit(-1);
            }
        }
#endif

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) {
            ExitEvent.Set();
            e.Cancel = true;
        }

        private static void PrintLogo() {
            /// Base64 + GZipped X3-JAM Logo
            string LogoZipBase64 = "H4sIAAAAAAAEAJWSUQ6AIAxD/028g8f" +
                "1ExKniYmX4yQGDFspQ6PZj1spj8IyT0v+0r7mkiPtsZT+rlyme" +
                "S8Uuw6uoTUrmZxMI1uSK/clWP2Ae5QR3MhhzFoWambR4BSLztG" +
                "MPoNsyVgPxGTukPGsboax6YiyVFZ+D6PMQGDXBWcGsvYG394Zx" +
                "gmFkXs7DYjx8BcK4J3ZDKScTfCx+nRV+eHcudX+DWQRQ5+dAwA" +
                "A";

            string logo = Unzip(Convert.FromBase64String(LogoZipBase64));

            Console.WriteLine(logo);
            Console.WriteLine("\nX3-JAM Server Developer-build (c) 2021 Estrol's Group Developers (Estrol and MatVeiQaa)");
            Console.WriteLine(string.Format("Current time is {0}\n", DateTime.Now));
            Console.WriteLine("Server Version: Pre-release 0.98 build #0976");

            if (IsWine()) {
                Console.WriteLine("ERROR: WINE DETECTED, Please use Linux build instead!!");
                Environment.Exit(-1);
            }

            Console.WriteLine();
        }

        private static void CopyTo(Stream src, Stream dest) {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
                dest.Write(bytes, 0, cnt);
            }
        }

        private static string Unzip(byte[] bytes) {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
                CopyTo(gs, mso);
            }

            return Encoding.UTF8.GetString(mso.ToArray());
        }

        public static bool IsWine() {
            if (!IsLinux) {
                int count = Process.GetProcessesByName("winlogon").Length;
                return count == 0;
            } else {
                return false;
            }
        }

        public static bool IsLinux {
            get {
                int p = (int)Environment.OSVersion.Platform;
                return (p == 4) || (p == 6) || (p == 128);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    }
}
