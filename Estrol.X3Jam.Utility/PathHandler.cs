using System.IO;
using System.Text.RegularExpressions;

namespace Estrol.X3Jam.Utility {
    public class PathHandler : EstrolUtilityBase {
        private PathHandler() { }

        public static string Handle(string rawPath) {
            string replaced = string.Format(rawPath, Path.DirectorySeparatorChar);

            return replaced;
        }
    }
}
