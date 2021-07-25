using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.IConsole {
    internal static class IUtils {
        public static void PrintLogo() {
            /// Base64 + GZipped X3-JAM Logo
            string LogoZipBase64 = "H4sIAAAAAAAEAJWSUQ6AIAxD/028g8f" +
                "1ExKniYmX4yQGDFspQ6PZj1spj8IyT0v+0r7mkiPtsZT+rlyme" +
                "S8Uuw6uoTUrmZxMI1uSK/clWP2Ae5QR3MhhzFoWambR4BSLztG" +
                "MPoNsyVgPxGTukPGsboax6YiyVFZ+D6PMQGDXBWcGsvYG394Zx" +
                "gmFkXs7DYjx8BcK4J3ZDKScTfCx+nRV+eHcudX+DWQRQ5+dAwA" +
                "A";

            string logo = Unzip(Convert.FromBase64String(LogoZipBase64));

            Console.WriteLine(logo);
            Console.WriteLine("\nX3-JAM Server (c) 2021 Estrol's Group Developers (Estrol and MatVeiQaa)");
            Console.WriteLine(string.Format("Current time is {0}\n", DateTime.Now));
            Console.WriteLine("Server Version: Release 1.0-r2 Build #1245");

            if (IsWine()) {
                Console.WriteLine("ERROR: WINE DETECTED, Please use Linux build instead!!");
                Environment.Exit(-1);
            }

            Console.WriteLine();
        }

        public static void CopyTo(Stream src, Stream dest) {
            byte[] bytes = new byte[4096];

            int cnt;

            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static string Unzip(byte[] bytes) {
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
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
    }
}
