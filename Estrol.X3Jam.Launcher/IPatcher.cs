using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Launcher {
    internal class Patcher {
        public static bool PatchExecutable(Process process) {
            try {
                List<byte[]> patches = PatchList();
                List<IntPtr> address = GetListAddrs(process);

                for (int i = 0; i < address.Count; i++) {
                    var patch = patches[i];
                    var addr = address[i];

                    WriteProcessMemory(process.Handle, addr, patch, patch.Length, out _);
                }
            } catch (Exception) {
                return false;
            }

            return true;
        }

        private static List<IntPtr> GetListAddrs(Process process) {
            IntPtr BaseAddr = process.Modules[0].BaseAddress;

            return new() {
                BaseAddr + 0x074AFA,
                BaseAddr + 0x07553E,
                BaseAddr + 0x08745A,
                BaseAddr + 0x087487,
                BaseAddr + 0x088EF7,
                BaseAddr + 0x088FF9,
                BaseAddr + 0x0891C0,
                BaseAddr + 0x0B9826
            };
        }

        private static List<byte[]> PatchList() {
            return new() {
                new byte[] { 0x90, 0x90 },
                new byte[] { 0x90, 0x90 },
                new byte[] { 0x83, 0xC4, 0x08, 0xC3, 0x90 },
                new byte[] { 0x83, 0xC4, 0x18, 0xC3, 0x90 },
                new byte[] { 0x83, 0xC4, 0x10, 0x31, 0xF6, 0x89, 0x04, 0x1E, 0x88, 0x4C, 0x1E, 0x04, 0x46, 0x83, 0xFE, 0x14, 0x7C, 0xF6, 0xEB, 0x04, 0x90, 0x90, 0x90, 0x90 },
                new byte[] { 0xEB, 0x01, 0x90, 0x52, 0xEB, 0x01, 0x90 },
                new byte[] { 0xC3, 0x90, 0x9 },
                new byte[] { 0xC3 },
            };
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int size, out IntPtr lpNumberOfBytesWritten);
    }
}
