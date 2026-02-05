using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ContractCreator.Shared.Helpers
{
    public static class JsonHelper
    {
        public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.General)
        {
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static string Serialize<T>(T source)
        {
            if (source == null) return string.Empty;

            return JsonSerializer.Serialize(source, Options);
        }

        public static T? Deserialize<T>(string source) where T : class
        {
            if (string.IsNullOrWhiteSpace(source)) return null;

            return JsonSerializer.Deserialize<T>(source, Options);
        }

        public static bool IsNullOrEmptyJson(string? jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString)) return true;
            if (jsonString == "{}") return true;
            if (string.IsNullOrWhiteSpace(jsonString.Replace("{", "").Replace("}", ""))) return true;

            return false;
        }
    }
}
