using System;
using System.Threading;
using System.IO;

using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website;
using Estrol.X3Jam.Utility;

namespace Estrol.X3Jam.IConsole {
    public class Program {
        private static ManualResetEvent ExitEvent = new(false);

        public static void Main(string[] args) {
            if (Type.GetType("Mono.Runtime") != null) {
                Console.WriteLine("ERROR: You cannot run this server app from linux or using mono, use linux build instead.");
                return;
            }

            Console.Title = "X3-JAM - Unofficial O2-JAM 1.8 Server";
            IUtils.PrintLogo();

#if RELEASE
            AppDomain.CurrentDomain.FirstChanceException += (caller, e) => {
                ExceptionHandler(new object(), e.Exception);
            };
#endif

            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "server.conf");
            bool IsMultiServer = false;
            if (File.Exists(path)) {
                ConfLoader conf = new(path);
                var value = conf.IniReadValue("CONFIG", "MultiServer");

                IsMultiServer = int.Parse(value) == 1;
            }

            if (IsMultiServer) {
                using Mutex mutex = new(false, "Global\\{X3JAM-ESTROL-GROUP-SIGNED}", out bool IsCreated);
                if (IsCreated) {
                    RunServer();
                } else {
                    if (!IUtils.IsLinux) {
                        _ = IUtils.MessageBox(IntPtr.Zero, "The server program already open!\nIf you want enable multiple server please set MultiServer to 1 in config!", "Error", (uint)0x00000010L);
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

            Console.CancelKeyPress += (caller, e) => {
                ExitEvent.Set();
                e.Cancel = true;
            };

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
    }
}
