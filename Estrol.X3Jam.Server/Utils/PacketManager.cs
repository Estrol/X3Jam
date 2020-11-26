﻿using Estrol.X3Jam.Server.Data;
using System;
using System.IO;

namespace Estrol.X3Jam.Server.Utils {
    public class PacketManager {
        public DateTime Time;
        public Packets opcode;
        public ushort _opcode;
        public byte[] data;

        private readonly MemoryStream ms;
        private readonly BinaryReader br;

        public PacketManager(byte[] rawData) {
            ms = new MemoryStream(rawData);
            br = new BinaryReader(ms);

            br.BaseStream.Seek(2, SeekOrigin.Begin);
            int timestamp = br.ReadInt32();
            Time = new DateTime(1970, 1, 1).AddSeconds(timestamp);

            br.BaseStream.Seek(26, SeekOrigin.Begin);
            int dataOffset = br.ReadInt32();
            int dataWithLength = br.ReadInt32();

            data = br.ReadBytes(dataOffset);
            _opcode = BitConverter.ToUInt16(data, 0);
            opcode = (Packets)_opcode;
        }
    }
}
