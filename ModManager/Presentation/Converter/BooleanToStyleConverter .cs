namespace ModManager.Presentation.Converter;

public class BooleanToStyleConverter : IValueConverter
{
    public Style TrueStyle { get; set; }
    public Style FalseStyle { get; set; }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue ? TrueStyle : FalseStyle;
        }

        return FalseStyle;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
