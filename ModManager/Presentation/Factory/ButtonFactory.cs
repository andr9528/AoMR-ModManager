namespace ModManager.Presentation.Factory;

public static class ButtonFactory
{
    public const string EDIT_SYMBOL_UNICODE = "\uE104";
    public const string DELETE_SYMBOL_UNICODE = "\uE107";
    public const string RENAME_SYMBOL_UNICODE = "\uE8AC";
    public const string LEFT_ARROW_SYMBOL_UNICODE = "\u2190";

    public static Button CreateDefaultButton()
    {
        return new Button
        {
            Margin = new Thickness(10),
            Padding = new Thickness(10, 5, 10, 5),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };
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
