using System;
using System.Threading;
using System.IO;
using System.IO.Compression;
using System.Text;

using Estrol.X3Jam.Server;
using Estrol.X3Jam.Website;
using System.Runtime.InteropServices;

namespace Estrol.X3Jam.IConsole {
    public class Program {
        static ManualResetEvent ExitEvent = new(false);

        static void Main(string[] args) {
            Type t = Type.GetType("Mono.Runtime");
            if (t != null) {
                Console.WriteLine("ERROR: You can't run this using mono, please use .NET 5 to run this!");
                return;
            }

            Console.Title = "X3Jam - O2-JAM 1.8 Server Emulation";
            PrintLogo();

            using Mutex mutex = new(true, "X3JAMSERVER", out bool createNew); 
            if (createNew) {
                O2JamServer Server = new();
                Server.Intialize();

                WebMain Website = new(Server, 15000);
                Website.Initalize();

                Console.CancelKeyPress += Console_CancelKeyPress;
                ExitEvent.WaitOne();
            } else {
#if _WINDOWS
                _ = MessageBox(IntPtr.Zero, "The server program already open!\nIf you want enable multiple server please set multi-server to 1 in config!", "Error", (uint)0x00000010L);
#else

                Console.WriteLine("ERROR: The server program already open!");
                Console.WriteLine("ERROR: If you want enable multi server please set multi-server to 1 in config");
                Console.WriteLine("ERROR: Press any key to continue");
                Console.ReadKey();
#endif
            }
        }

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
            Console.WriteLine("\nX3Jam Server Developer version (c) 2021 Estrol's Group Developers (Estrol and MatVeiQaa)");
            Console.WriteLine(string.Format("Current time is {0}\n", DateTime.Now));
            Console.WriteLine("Server Version: alpha-0.3 build #0012");
            Console.WriteLine();
        }

        private static void CopyTo(Stream src, Stream dest)
        {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0)
            {
                dest.Write(bytes, 0, cnt);
            }
        }

        private static string Unzip(byte[] bytes)
        {
            using var msi = new MemoryStream(bytes);
            using var mso = new MemoryStream();
            using (var gs = new GZipStream(msi, CompressionMode.Decompress))
            {
                CopyTo(gs, mso);
            }

            return Encoding.UTF8.GetString(mso.ToArray());
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
    }
}
