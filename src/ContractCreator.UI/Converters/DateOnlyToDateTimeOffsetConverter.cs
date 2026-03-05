namespace ContractCreator.UI.Converters
{
    public class DateOnlyToDateTimeOffsetConverter : IValueConverter
    {
        public static readonly DateOnlyToDateTimeOffsetConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateOnly dateOnly)
                return dateOnly.ToDateTime(TimeOnly.MinValue);

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
                return DateOnly.FromDateTime(dateTime);

            return null;
        }
    }
}
