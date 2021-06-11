using System;
using System.IO;
using System.Linq;
using System.Text;
using Estrol.X3Jam.Utility;
using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Server.CManager;

namespace Estrol.X3Jam.Server.CHandler {
    public abstract class CBase {
        public Client Client { get; private set; } 
        public O2JamServer Main { get; private set; }
        public MemoryStream Stream { get; private set; }
        public BinaryWriter Writer { get; private set; }
        public RoomManager RoomManager { get; private set; }
        public ChanManager ChanManager { get; private set; }

        public bool PrintTheResult = false;

        /// <summary>
        /// Intialize Client handler
        /// </summary>
        public CBase(Client client) {
            Intialize(client, true);
        }

        public CBase(Client client, bool stream) {
            Intialize(client, stream);
        }

        public void Intialize(Client client, bool stream) {
            Client = client;
            Main = Client.Main;
            ChanManager = Client.Main.ChannelManager;
            if (Client.UserInfo != null && Client.UserInfo.ChannelID != -1) {
                RoomManager = ChanManager.GetChannelByID(Client.UserInfo.ChannelID).RManager;
            }

            if (stream) {
                Stream = new MemoryStream(8192);
                Writer = new BinaryWriter(Stream);
            }
        }

        public abstract void Code();

        /// <summary>
        /// Execute Code() function;
        /// </summary>
        public void Handle() => Code();

        /// <summary>
        /// Log the Send() output to local file to debug
        /// </summary>
        /// <param name="contents"></param>
        private static void Print(byte[] contents) {
            string stringify = Hexdump.HexDump(contents);
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Debug"))) {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Debug"));
            }

            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Debug", $"{Guid.NewGuid()}.txt"), stringify);
        }

        /// <summary>
        /// Send the data with length of 2 first byte data
        /// </summary>
        public void Send() {
            byte[] data = Stream.ToArray();
            short length = BitConverter.ToInt16(data, 0);
            TSend(data, length);
        }

        /// <summary>
        /// Send the data with specific length
        /// </summary>
        public void Send(short length) {
            byte[] data = Stream.ToArray();
            TSend(data, length);
        }

        public void Send(byte[] data) {
            short length = BitConverter.ToInt16(data, 0);
            Send(data, length);
        }

        public void Send(byte[] data, short length) => TSend(data, length);

        private void TSend(byte[] data, short length) {
            if (PrintTheResult) {
                Print(data);
            }

            Client.Send(data, length);
        }

        public byte[] ToArray() {
            return Stream.ToArray();
        }

        // Single data types

        /// <summary>
        /// Write short into array
        /// </summary>
        /// <param name="val"></param>
        public void Write(short val) => Writer.Write(val);

        /// <summary>
        /// Write int32 to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(int val) => Writer.Write(val);

        /// <summary>
        /// Write double to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(double val) => Writer.Write(val);

        /// <summary>
        /// Write char to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(char val) => Writer.Write(val);

        /// <summary>
        /// Write byte to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(byte val) => Writer.Write(val);

        // Array data types

        /// <summary>
        /// Write byte[] to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(byte[] val) => Writer.Write(val);

        /// <summary>
        /// Write char[] to array
        /// </summary>
        /// <param name="val"></param>
        public void Write(char[] val) => Writer.Write(val);

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
            Writer.Write(bData);
        }

        /// <summary>
        /// Set first 2 byte of the stream length into stream
        /// </summary>
        public void SetL() => SetL((int)Stream.Length);

        /// <summary>
        /// Set first 2 byte of the stream length into stream
        /// </summary>
        public void SetL(int length) {
            Writer.Seek(0, SeekOrigin.Begin);
            Writer.Write((short)length);

            Writer.Seek((int)Stream.Length, SeekOrigin.End);
        }
    }
}
