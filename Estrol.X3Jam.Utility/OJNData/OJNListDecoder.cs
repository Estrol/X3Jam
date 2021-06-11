// Author: SirusDoma
// License: MIT
// Code: https://github.com/SirusDoma/O2MusicList

using System.IO;
using System.Text;

namespace Estrol.X3Jam.Utility {
    public static class OJNListDecoder {
        private const string ChineseEncoding = "big5";

        public static OJNList Decode(string FileName) {
            return Decode(File.Open(FileName, FileMode.Open), false);
        }

        public static OJNList Decode(Stream stream, bool keepOpen = false) {
            var headers = new OJNList();

            using var reader = new BinaryReader(stream, Encoding.Unicode, keepOpen); stream.Seek(0, SeekOrigin.Begin);
            int songCount = reader.ReadInt32();

            headers.Version = stream.Length > 4 + (songCount * 300) ? OJNFileFormat.New : OJNFileFormat.Old;
            var charset = ChineseEncoding;

            for (int i = 0; i < songCount; i++) {
                headers.Add(OJNDecoder.Decode(reader.ReadBytes(300), Encoding.GetEncoding(charset)));
            }

            return headers;
        }
    }
}
