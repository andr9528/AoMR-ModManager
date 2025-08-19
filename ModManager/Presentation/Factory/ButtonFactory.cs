namespace ModManager.Presentation.Factory;

public static class ButtonFactory
{
    public static Button CreateDefaultButton()
    {
        var button = new Button
        {
            Margin = new Thickness(10),
            Padding = new Thickness(10, 5, 10, 5),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            IsTabStop = false,
        };

        button.Style(Theme.Button.Styles.Filled);

        return button;
    }

    public static Button CreateFontIconButton(string glyphUnicode, int fontSize = 12)
    {
        Button button = CreateDefaultButton();

        var icon = new FontIcon()
        {
            FontSize = fontSize,
            Glyph = glyphUnicode,
        };

        button.Content = icon;

        return button;
    }
}
