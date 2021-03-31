using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Server.Utils {
    public static class DataUtils {
        public static IEnumerable<IEnumerable<T>> Split<T>(this T[] array, int size) {
            for (var i = 0; i < (float)array.Length / size; i++) {
                yield return array.Skip(i * size).Take(size);
            }
        }

        public static string[] GetUserAuthentication(byte[] data) {
            int length = 0;
            for (int i = 0; i < data.Length; i++) {
                if (data[i + 2] == 0x00) {
                    length = i;
                    break;
                }
            }

            byte[] buf = new byte[length];
            Buffer.BlockCopy(data, 2, buf, 0, length);

            byte[] dump = new byte[data.Length - (length + 3)];
            Buffer.BlockCopy(data, length + 3, dump, 0, data.Length - (length + 3));
            int length2 = 0;
            for (int i = 0; i < dump.Length; i++) {
                if (dump[i] == 0x00) {
                    length2 = i;
                    break;
                }
            }

            byte[] buf2 = new byte[length2];
            Buffer.BlockCopy(dump, 0, buf2, 0, length2);

            return new string[] {
                Encoding.UTF8.GetString(buf),
                Encoding.UTF8.GetString(buf2)
            };
        }

        public static string GetString(byte[] data) {
            int length = 0;
            for (int i = 0; i < data.Length; i++) {
                if (data[i + 2] == 0x00) {
                    length = i;
                    break;
                }
            }

            byte[] buf = new byte[length];
            Buffer.BlockCopy(data, 2, buf, 0, length);

            return Encoding.UTF8.GetString(buf);
        }

        public static TV GetValue<TK, TV>(this Dictionary<TK, TV> dict, TK key, TV defaultValue = default) {
            return dict.TryGetValue(key, out TV value) ? value : defaultValue;
        }

        public static T Next<T>(this T src) where T : struct {
            if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }
}
