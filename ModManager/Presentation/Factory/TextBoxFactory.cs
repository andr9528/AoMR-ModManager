namespace ModManager.Presentation.Factory;

public static class TextBoxFactory
{
    public static TextBox CreateDefaultTextBox()
    {
        var box = new TextBox
        {
            Margin = new Thickness(10),
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Center,
            FontSize = 14,
        };
        return box;
    }
}
