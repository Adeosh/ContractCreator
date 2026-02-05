using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;

namespace ContractCreator.Shared.Helpers.Extensions
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public abstract class CustomAttribute : Attribute
    {
        public object Value { get; }
        protected CustomAttribute(object value) => Value = value;
    }

    public class ExternalDescriptionAttribute : CustomAttribute
    {
        public ExternalDescriptionAttribute(string value) : base(value) { }
    }

    public static class EnumsExtension
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<object, string>> _descriptionCache = new();
        private static readonly ConcurrentDictionary<(Type EnumType, Type AttrType), Dictionary<object, string>> _customAttrCache = new();

        /// <summary> Получить значение атрибута [Description] из Enum значения. Использует кэширование. </summary>
        public static string GetDescription<T>(this T enumValue) where T : struct, Enum
        {
            Type type = typeof(T);
            Dictionary<object, string> map = _descriptionCache.GetOrAdd(type, t => InitializeDescriptionCache(t));

            return map.TryGetValue(enumValue, out var desc) ? desc : enumValue.ToString();
        }

        /// <summary> Получить значение кастомного атрибута-наследника CustomAttribute. </summary>
        public static string? GetCustomAttributeDescription<TAttr>(this Enum enumValue)
            where TAttr : CustomAttribute
        {
            Type enumType = enumValue.GetType();
            Type attrType = typeof(TAttr);

            Dictionary<object, string> map = _customAttrCache.GetOrAdd((enumType, attrType), key => InitializeCustomAttributeCache(key.EnumType, key.AttrType));

            return map.TryGetValue(enumValue, out var val) ? val : null;
        }

        /// <summary> Получить список пар (Значение Enum, Описание). </summary>
        public static List<KeyValuePair<T, string>> GetDescriptionsList<T>(this T enumObj, bool withZeroValue = true)
            where T : struct, Enum
        {
            return Enum.GetValues<T>()
                .Where(e => withZeroValue || Convert.ToInt64(e) != 0)
                .Select(e => new KeyValuePair<T, string>(e, e.GetDescription()))
                .ToList();
        }

        /// <summary> Получить список значений Enum. </summary>
        public static List<T> GetItems<T>(this T enumObj, bool withZeroValue = true)
            where T : struct, Enum
        {
            return Enum.GetValues<T>()
                .Where(e => withZeroValue || Convert.ToInt64(e) != 0)
                .ToList();
        }

        /// <summary> Проверка значения на вхождение в Enum. </summary>
        public static bool CheckIsDefined<T>(this T enumValue) where T : struct, Enum
        {
            return Enum.IsDefined(enumValue);
        }

        private static Dictionary<object, string> InitializeDescriptionCache(Type enumType)
        {
            return Enum.GetValues(enumType)
                .Cast<object>()
                .ToDictionary(
                    val => val,
                    val =>
                    {
                        FieldInfo? field = enumType.GetField(val.ToString()!);
                        DescriptionAttribute? attr = field?.GetCustomAttribute<DescriptionAttribute>();
                        return attr?.Description ?? val.ToString()!;
                    }
                );
        }

        private static Dictionary<object, string> InitializeCustomAttributeCache(Type enumType, Type attrType)
        {
            Dictionary<object, string> result = new Dictionary<object, string>();

            foreach (var val in Enum.GetValues(enumType))
            {
                FieldInfo? field = enumType.GetField(val.ToString()!);
                CustomAttribute? attr = field?.GetCustomAttributes(attrType, false).FirstOrDefault() as CustomAttribute;

                if (attr != null && attr.Value != null)
                    result[val] = attr.Value.ToString()!;
            }
            return result;
        }
    }
}
