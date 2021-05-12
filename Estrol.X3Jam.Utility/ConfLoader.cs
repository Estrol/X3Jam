using IniParser;
using IniParser.Model;

/// <summary>
/// This Wrapper meant to replace Win32 api that I usually use to able crossplatform without wine
/// </summary>
namespace Estrol.X3Jam.Utility {
    public class ConfLoader : EstrolUtilityBase {
        private FileIniDataParser Parser;
        private IniData Data;
        private string FileName;

        public ConfLoader(string fileName) {
            Parser = new();
            Data = Parser.ReadFile(fileName);
            FileName = fileName;
        }

        public string IniReadValue(string Section, string Key) {
            return Data[Section][Key];
        }

        public void IniWriteValue(string Section, string Key, string KeyData) {
            Data[Section][Key] = KeyData;
            Parser.WriteFile(FileName, Data);
        }
    }
}
