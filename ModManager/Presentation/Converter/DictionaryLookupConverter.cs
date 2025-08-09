namespace ModManager.Presentation.Converter;

public class DictionaryLookupConverter<TKey, TValue> : IValueConverter
{
    public IDictionary<TKey, TValue> Dictionary { get; set; }

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (Dictionary == null)
        {
            throw new ArgumentNullException(nameof(Dictionary), "Dictionary cannot be null.");
        }

        if (value is TKey key && Dictionary.TryGetValue(key, out TValue? result))
        {
            return result ?? DependencyProperty.UnsetValue; // Return UnsetValue if the value is null
        }

        return DependencyProperty.UnsetValue;
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
