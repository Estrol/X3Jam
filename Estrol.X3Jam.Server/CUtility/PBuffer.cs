using System.Linq;
using System.IO;
using System.Text;
using System;

namespace Estrol.X3Jam.Server.Utils {
    public class PBuffer {
        private MemoryStream m_ms;
        private BinaryWriter m_bw;

        public PBuffer() {
            BufferIntialize(8192);
        }

        public PBuffer(int capacity) {
            BufferIntialize(capacity);
        }

        public void SetL() => SetL((int)m_ms.Length);

        public void SetL(int length) {
            m_bw.Seek(0, SeekOrigin.Begin);
            m_bw.Write((short)length);

            m_bw.Seek((int)m_ms.Length, SeekOrigin.End);
        }

        public byte[] ToArray() {
            return m_ms.ToArray();
        }

        private void BufferIntialize(int c) {
            m_ms = new MemoryStream(c);
            m_bw = new BinaryWriter(m_ms);
        }

        public void WriteB(bool val)    => m_bw.Write(val);

        public void WriteI(int val)     => m_bw.Write(val);

        public void WriteL(long val)    => m_bw.Write(val);

        public void WriteD(double val)  => m_bw.Write(val);
         
        public void WriteBB(byte val)   => m_bw.Write(val);

        public void WriteS(short val)   => m_bw.Write(val);

        public void WriteBA(byte[] val) => m_bw.Write(val);

        public void WriteStr(string val) {
            WriteStr(val, Encoding.UTF8);
        }

        public void WriteStr(string val, Encoding encoding) {
            char[] data = val.ToCharArray();
            data = data.Concat(new char[1]).ToArray();

            byte[] bData = encoding.GetBytes(data);
            m_bw.Write(bData);
        }
    }
}
