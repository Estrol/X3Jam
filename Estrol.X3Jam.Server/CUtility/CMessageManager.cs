using Estrol.X3Jam.Server.CData;
using Estrol.X3Jam.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Estrol.X3Jam.Server.CUtility {
    public class CMessageManager {
        public bool IsFailed = false;
        public List<byte[]> stacks = new();
        public List<CMessage> packets = new();

        public CMessageManager(Client client, byte[] rawData) {
            HandleMultiplePacket(rawData);

            foreach (byte[] data in stacks) {
                packets.Add(new(this, client, data));
            }
        }

        public void HandleMultiplePacket(byte[] rawPacket) {
            using MemoryStream ms = new(rawPacket);
            using BinaryReader br = new(ms);

            while (ms.Position < ms.Length) {
                short len = br.ReadInt16();
                br.BaseStream.Seek(br.BaseStream.Position - 2, SeekOrigin.Begin);
                byte[] data = br.ReadBytes(len);

                stacks.Add(data);
            }
        }
    }
}
