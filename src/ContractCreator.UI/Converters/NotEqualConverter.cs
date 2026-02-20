namespace ContractCreator.UI.Converters
{
    public class NotEqualConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null || values.Count < 2) return false;

            var val1 = values[0]?.ToString();
            var val2 = values[1]?.ToString();

            return val1 != val2;
        }
    }
}
