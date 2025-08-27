namespace ModManager.Presentation;

public static class ComboBoxFactory
{
    public static ComboBox CreateDefaultComboBox(params string[] options)
    {
        var comboBox = new ComboBox
        {
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 14,
            ItemsSource = options,
        };
        return comboBox;
    }
}
