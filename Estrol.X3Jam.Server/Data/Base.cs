using Estrol.X3Jam.Server;
using Estrol.X3Jam.Server.Data;
using System;
using System.IO;

namespace Estrol.X3Jam.Server.Data {
    public class Base {
        public MemoryStream ms;
        public BinaryWriter bw;
        public Connection state;

        public Base(Connection state) {
            this.state = state;

            ms = new MemoryStream();
            bw = new BinaryWriter(ms);
        }

        /// <summary>
        /// Override the packet length
        /// </summary>
        /// <param name="length"></param>
        public void SetLength(short length) {
            bw.Seek(0, SeekOrigin.Begin);
            bw.Write(length);
            bw.Seek(ms.ToArray().Length, SeekOrigin.End);
        }

        /// <summary>
        /// Copy the array then return it
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray() {
            MemoryStream _ms = new MemoryStream();
            ms.CopyTo(_ms);

            return _ms.ToArray();
        }

        /// <summary>
        /// Write buffer from Connection#Buffer
        /// </summary>
        /// <param name="length"></param>
        public void WriteFromLength(short length) {
            byte[] tmpBuffer = new byte[length];
            Buffer.BlockCopy(state.Buffer, 0, tmpBuffer, 0, length);

            bw.Write(tmpBuffer);
        }

        /// <summary>
        /// Write data
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte data) {
            bw.Write(data);
        }

        /// <summary>
        /// Write data
        /// </summary>
        /// <param name="data"></param>
        public void Write(short data) {
            bw.Write(data);
        }

        /// <summary>
        /// Write data
        /// </summary>
        /// <param name="data"></param>
        public void Write(byte[] data) {
            bw.Write(data);
        }

        /// <summary>
        /// Send the data (with specific length)
        /// </summary>
        /// <param name="length"></param>
        public void Send(short length = 0) {
            byte[] data = ms.ToArray();

            if (length == 0) {
                length = BitConverter.ToInt16(data, 0);
            }

            state.Send(data, length);
        }
    }
}
