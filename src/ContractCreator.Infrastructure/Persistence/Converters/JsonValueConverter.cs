using ContractCreator.Shared.Helpers;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace ContractCreator.Infrastructure.Persistence.Converters
{
    /// <summary> Конвертация из объекта в JSON строку и обратно </summary>
    public class JsonValueConverter<T> : ValueConverter<T?, string?>
        where T : class
    {
        public JsonValueConverter()
            : base(
                j => j == null ? null : JsonSerializer.Serialize(j, JsonHelper.Options),
                j => string.IsNullOrEmpty(j) ? null : JsonSerializer.Deserialize<T>(j, JsonHelper.Options)
            )
        { }
    }
}
