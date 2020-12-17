using System;
using System.IO;
using System.Linq;
using System.Text;
using Estrol.X3Jam.Server.CData;

namespace Estrol.X3Jam.Server.CHandler {
    public abstract class CBase {
        private MemoryStream m_stream;
        private BinaryWriter m_writer;
        public Client m_client;

        public Client Client => m_client;
        public MemoryStream Stream => m_stream;
        public BinaryWriter Writer => m_writer;

        /// <summary>
        /// Intialize Client handler
        /// </summary>
        public CBase(Client client) {
            m_client = client;

            m_stream = new MemoryStream(8192);
            m_writer = new BinaryWriter(m_stream);
        }

        public CBase(Client client, bool stream) {
            m_client = client;

            if (stream) {
                m_stream = new MemoryStream(8192);
                m_writer = new BinaryWriter(m_stream);
            }
        }

        public abstract void Code();

        public void Handle() {
#if DEBUG
            Code();
#else
            try {
                Code();
            } catch (Exception) {
                throw;
            }
#endif
        }
        
        /// <summary>
        /// Send the data with length of 2 first byte data
        /// </summary>
        public void Send() {
            byte[] data = m_stream.ToArray();
            short length = BitConverter.ToInt16(data, 0);
            _Send(data, length);
        }

        /// <summary>
        /// Send the data with specific length
        /// </summary>
        public void Send(short length) {
            byte[] data = m_stream.ToArray();
            _Send(data, length);
        }

        private void _Send(byte[] data, short length) {
            m_client.Send(data, length);
        }

        public void Send(byte[] data) {
            short length = BitConverter.ToInt16(data, 0);
            Send(data, length);
        }

        public void Send(byte[] data, short length) {
            m_client.Send(data, length);
        }

        // Single data types
        
        /// <summary>
        /// Write short into array
        /// </summary>
        /// <param name="val"></param>
        public void Write(short val) => m_writer.Write(val);

        /// <summary>
        /// Write int32 to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(int val) => m_writer.Write(val);

        /// <summary>
        /// Write double to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(double val) => m_writer.Write(val);

        /// <summary>
        /// Write char to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(char val) => m_writer.Write(val);

        /// <summary>
        /// Write byte to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(byte val) => m_writer.Write(val);

        // Array data types

        /// <summary>
        /// Write byte[] to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(byte[] val) => m_writer.Write(val);

        /// <summary>
        /// Write char[] to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(char[] val) => m_writer.Write(val);

        // String based type

        /// <summary>
        /// Write UTF-8 string null terminated to array
        /// </summary>
        /// <param name="str"></param>
        public void Write(string str) => Write(str, Encoding.UTF8);

        /// <summary>
        /// Write string null terminated to array with specific Encoding
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        public void Write(string str, Encoding encoding) {
            char[] data = str.ToCharArray();
            data = data.Concat(new char[1]).ToArray();

            byte[] bData = encoding.GetBytes(data);
            m_writer.Write(bData);
        }

        public void SetL() => SetL((int)m_stream.Length);

        public void SetL(int length) {
            m_writer.Seek(0, SeekOrigin.Begin);
            m_writer.Write((short)length);

            m_writer.Seek((int)m_stream.Length, SeekOrigin.End);
        }
    }
}
