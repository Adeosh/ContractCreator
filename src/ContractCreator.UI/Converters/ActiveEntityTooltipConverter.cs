namespace ContractCreator.UI.Converters
{
    public class ActiveEntityTooltipConverter : IMultiValueConverter
    {
        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values == null || values.Count < 2) return "Удалить";

            var val1 = values[0]?.ToString();
            var val2 = values[1]?.ToString();

            if (val1 == val2)
                return parameter?.ToString() ?? "Удаление активного элемента запрещено";

            return "Удалить";
        }
    }
}
