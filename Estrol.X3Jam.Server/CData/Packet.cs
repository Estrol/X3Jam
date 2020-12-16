using Estrol.X3Jam.Server;
using Estrol.X3Jam.Server.CData;
using System;
using System.IO;

namespace Estrol.X3Jam.Server.CData {
    public class Packet {
        public MemoryStream m_MemoryStream;
        public BinaryWriter m_BinaryWriter;
        public Client m_connection;

        public Packet(Client state) {
            m_connection = state;

            m_MemoryStream = new MemoryStream();
            m_BinaryWriter = new BinaryWriter(m_MemoryStream);
        }

        /// <summary>
        /// Override the packet length
        /// </summary>
        /// <param name="length"></param>
        public void SetLength(short length) {
            m_BinaryWriter.Seek(0, SeekOrigin.Begin);
            m_BinaryWriter.Write(length);
            m_BinaryWriter.Seek(m_MemoryStream.ToArray().Length, SeekOrigin.End);
        }

        /// <summary>
        /// Copy the array then return it
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray() {
            MemoryStream _ms = new MemoryStream();
            m_MemoryStream.CopyTo(_ms);

            return _ms.ToArray();
        }

        /// <summary>
        /// Write buffer from Connection#Buffer
        /// </summary>
        /// <param name="length"></param>
        public void WriteFromLength(short length) {
            byte[] tmpBuffer = new byte[length];
            Buffer.BlockCopy(m_connection.Buffer, 0, tmpBuffer, 0, length);

            m_BinaryWriter.Write(tmpBuffer);
        }

        /// <summary>
        /// Write data
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte data) {
            m_BinaryWriter.Write(data);
        }

        /// <summary>
        /// Write data
        /// </summary>
        /// <param name="data"></param>
        public void Write(short data) {
            m_BinaryWriter.Write(data);
        }

        /// <summary>
        /// Write data
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data) {
            m_BinaryWriter.Write(data);
        }

        /// <summary>
        /// Send the data (with specific length)
        /// </summary>
        /// <param name="length"></param>
        public void Send(short length = 0) {
            byte[] data = m_MemoryStream.ToArray();

            if (length == 0) {
                length = BitConverter.ToInt16(data, 0);
            }

            m_connection.Send(data, length);
        }
    }
}
