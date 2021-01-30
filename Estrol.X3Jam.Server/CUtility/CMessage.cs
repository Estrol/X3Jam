using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.IO;
using System.Linq;

namespace Estrol.X3Jam.Server.Utils {
    public class CMessage {
        public DateTime Time;
        public ClientPacket opcode;
        public ushort _opcode;
        public byte[] data;
        public byte[] full_data { set; get; }

        private readonly MemoryStream ms;
        private readonly BinaryReader br;

        public bool IsFailed = false;

        public CMessage(Client client, byte[] rawData) {
            ms = new(rawData);
            br = new(ms);

            try {
                br.BaseStream.Seek(2, SeekOrigin.Begin);
                int timestamp = br.ReadInt32();
                Time = new DateTime(1970, 1, 1).AddSeconds(timestamp);

                br.BaseStream.Seek(26, SeekOrigin.Begin);
                int dataOffset = br.ReadInt32();
                int dataWithLength = br.ReadInt32();

                data = br.ReadBytes(dataOffset);
                byte[] bLen = BitConverter.GetBytes((short)dataWithLength);
                full_data = bLen.Concat(data).ToArray();

                _opcode = BitConverter.ToUInt16(data, 0);
                opcode = (ClientPacket)_opcode;
            } catch (Exception error) {
                IsFailed = true;
                if (error.Message == "Unable to read beyond the end of the stream.") {
                    Log.Write("[{0}@{1}] Client disconnected with abnormal way.", client.UserInfo.Username, client.IPAddr);
                } else {
                    throw;
                }
            }
        }
    }
}
