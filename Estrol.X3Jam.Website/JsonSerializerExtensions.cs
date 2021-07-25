using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Estrol.X3Jam.Website {
    public static partial class JsonSerializerExtensions {
        public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject, JsonSerializerOptions options = default)
            => JsonSerializer.Deserialize<T>(json, options);

        public static ValueTask<TValue> DeserializeAnonymousTypeAsync<TValue>(Stream stream, TValue anonymousTypeObject, JsonSerializerOptions options = default, CancellationToken cancellationToken = default)
            => JsonSerializer.DeserializeAsync<TValue>(stream, options, cancellationToken); // Method to deserialize from a stream added for completeness

        public static string Serialize<T>(T obj) => JsonSerializer.Serialize(obj);
        public static T Deserialize<T>(string json) => JsonSerializer.Deserialize<T>(json);
    }
}
