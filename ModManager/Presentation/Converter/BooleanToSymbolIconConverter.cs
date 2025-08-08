namespace ModManager.Presentation.Converter;

public class BooleanToSymbolIconConverter : IValueConverter
{
    // Optional: Use symbols that make sense for your UI state
    public Symbol TrueSymbol { get; set; } = Symbol.ClosePane;
    public Symbol FalseSymbol { get; set; } = Symbol.OpenPane;

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue ? new SymbolIcon(TrueSymbol) : new SymbolIcon(FalseSymbol);
        }

        return FalseSymbol;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is Symbol symbol)
        {
            return symbol == TrueSymbol;
        }

        return false;
    }
}
