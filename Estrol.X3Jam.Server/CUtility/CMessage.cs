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
        public bool IsHTTP;
        public bool IsFailed;

        public string[] HTTPMethod = {
            "GET",
            "HEAD",
            "POST",
            "PUT",
            "DELETE",
            "CONNECT",
            "OPTIONS",
            "TRACE",
            "PATCH"
        };

        private readonly MemoryStream ms;
        private readonly BinaryReader br;

        public CMessage(CMessageManager Base, Client client, byte[] rawData) {
            ms = new(rawData);
            br = new(ms);

            try {
                // This is needed to check whatever the request is HTTP or not
                string sData = Encoding.UTF8.GetString(ms.ToArray());
                string[] RawHeaderSeperator = new string[] { "\r\n" };
                string HttpHeader = sData.Split(RawHeaderSeperator, StringSplitOptions.RemoveEmptyEntries)[0];
                string HttpData = HttpHeader.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[0];

                if (HttpData != "\0" && HTTPMethod.Contains(HttpData)) {
                    IsHTTP = true;
                }

                br.BaseStream.Seek(2, SeekOrigin.Begin);
                int timestamp = br.ReadInt32();
                Time = new DateTime(1970, 1, 1).AddSeconds(timestamp);

                br.BaseStream.Seek(26, SeekOrigin.Begin);
                int dataOffset = br.ReadInt32();
                int dataWithLength = br.ReadInt32();

                Data = br.ReadBytes(dataOffset);
                byte[] bLen = BitConverter.GetBytes((short)dataWithLength);
                FullData = bLen.Concat(Data).ToArray();

                Opcode = (ClientPacket)BitConverter.ToUInt16(Data, 0);
            } catch (Exception error) {
                if (error is ArgumentOutOfRangeException) {
                    IsFailed = true;
                    return;
                }

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
