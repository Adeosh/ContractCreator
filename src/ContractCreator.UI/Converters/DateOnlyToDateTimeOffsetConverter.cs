namespace ContractCreator.UI.Converters
{
    public class DateOnlyToDateTimeOffsetConverter : IValueConverter
    {
        public static readonly DateOnlyToDateTimeOffsetConverter Instance = new();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateOnly date)
            {
                return new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue));
            }
            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return DateOnly.FromDateTime(dateTimeOffset.DateTime);
            }
            return null;
        }
    }
}
