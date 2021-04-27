using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.CUtility {
    public class CMessage {
        public DateTime Time;
        public ClientPacket Opcode;
        public byte[] Data;
        public byte[] FullData;

        private readonly MemoryStream ms;
        private readonly BinaryReader br;

        public ushort _opcode => (ushort)Opcode;
        public ClientPacket opcode => Opcode;
        public byte[] full_data => FullData;
        public byte[] data => Data;

        public CMessage(CMessageManager Base, Client client, byte[] rawData) {
            ms = new(rawData);
            br = new(ms);

            try {
                br.BaseStream.Seek(2, SeekOrigin.Begin);
                int timestamp = br.ReadInt32();
                Time = new DateTime(1970, 1, 1).AddSeconds(timestamp);

                br.BaseStream.Seek(26, SeekOrigin.Begin);
                int dataOffset = br.ReadInt32();
                int dataWithLength = br.ReadInt32();

                Data = br.ReadBytes(dataOffset);
                byte[] bLen = BitConverter.GetBytes((short)dataWithLength);
                FullData = bLen.Concat(data).ToArray();

                Opcode = (ClientPacket)BitConverter.ToUInt16(data, 0);
            } catch (Exception error) {
                Base.IsFailed = true;
                if (error.Message == "Unable to read beyond the end of the stream.") {
                    Log.Write("[{0}@{1}] Client disconnected with abnormal way.", client.UserInfo.Username, client.IPAddr);
                } else {
                    throw;
                }
            }
        }
    }
}
