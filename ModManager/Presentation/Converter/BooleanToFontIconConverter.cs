namespace ModManager.Presentation.Converter;

public class BooleanToFontIconConverter : IValueConverter
{
    public string TrueGlyph { get; set; }
    public string FalseGlyph { get; set; }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue ? new FontIcon() {Glyph = TrueGlyph,} : new FontIcon() {Glyph = FalseGlyph,};
        }

        return new FontIcon() {Glyph = FalseGlyph,};
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
