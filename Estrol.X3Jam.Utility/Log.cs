using System;
using System.Diagnostics;

namespace Estrol.X3Jam.Utility {
    public class Log : EstrolUtilityBase {
        private Log() {
            throw new NotImplementedException("This class is static class");
        }

        public static void Write(object content) {
            OutputToConsole($"{content}");
        }


        public static void Write(string content) {
            OutputToConsole(content);
        }

        public static void Write(string content, params object[] objects) {
            content = string.Format(content, objects);

            OutputToConsole(content);
        }

        private static void OutputToConsole(string content) {
            string time = DateTime.Now.ToString("HH:mm:ss:ff");

            Console.WriteLine("[{0}] {1}", time, content);
            Debug.WriteLine("[{0}] {1}", time, content);
        }
    }
}
