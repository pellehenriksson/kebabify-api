using System.Text.Json;

namespace Kebabify.Api.Common
{
    public static class KebabifyExtensions
    {
        private static readonly JsonSerializerOptions options = new();

        public static string ToJson<T>(this T item)
        {
            return JsonSerializer.Serialize(item, options);
        }
    }
}
